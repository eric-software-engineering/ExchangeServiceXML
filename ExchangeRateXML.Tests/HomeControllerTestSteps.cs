using ExchangeRateXML.Controllers;
using ExchangeRateXML.Interfaces;
using ExchangeRateXML.Models;
using ExchangeRateXML.Models.DTOs;
using ExchangeRateXML.Models.RedisCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechTalk.SpecFlow;

namespace ExchangeRateXML.Tests
{
  [Binding]
  public class HomeControllerTestSteps
  {
    private HomeController _appController;
    private Mock<IRepository<IHTTPClientAdapter>> _apiClientMock;
    private Mock<IRepository<AzureRedisControllerCache>> _cacheRepositoryMock;
    private readonly ExchangeRate _rate = new ExchangeRate { baseCurrency = "USD", targetCurrency = "AUD", exchangeRate = 2, timestamp = new DateTime(2000, 1, 1) };
    private ActionResult result;
    private readonly List<Currency> _currencies = new List<Currency> { new Currency { Code = "USD" }, new Currency { Code = "AUD" }, new Currency { Code = "EUR" } };

    [Given(@"there is a Home Controller")]
    public void GivenThereIsAHomeController()
    {
      _apiClientMock = new Mock<IRepository<IHTTPClientAdapter>>();
      _cacheRepositoryMock = new Mock<IRepository<AzureRedisControllerCache>>();

      _appController = new HomeController(_cacheRepositoryMock.Object, _apiClientMock.Object);
    }

    [Given(@"the cache is not null or outdated")]
    public void GivenTheCacheIsNotNullOrOutdated()
    {
      DateTime? time = DateTime.Now.AddMinutes(-5);
      _cacheRepositoryMock.Setup(x => x.GetData("USD", "AUD")).Returns(Task.FromResult(_rate));
      _cacheRepositoryMock.Setup(x => x.GetCurrencies()).Returns(Task.FromResult(_currencies));
      _cacheRepositoryMock.Setup(x => x.GetTimeStamp()).Returns(Task.FromResult(time));
    }

    [When(@"the user accesses the Index page")]
    public async void WhenTheUserAccessesTheIndexPage()
    {
      result = await _appController.Index();
    }

    [Then(@"the currencies should be returned")]
    public void ThenTheCurrenciesShouldBeReturned()
    {
      Assert.AreEqual(((ViewResultBase)result).ViewData.Model, _currencies);
    }

    [Given(@"the cache is null or outdated")]
    public void GivenTheCacheIsNullOrOutdated()
    {
      // Nothing to change here
    }

    [Then(@"XML should be called")]
    public void ThenXMLShouldBeCalled()
    {
      _apiClientMock.Verify(x => x.GetAllData());
    }

    [When(@"the user requests a conversion of the same currencies")]
    public async void WhenTheUserRequestsAConversionOfTheSameCurrencies()
    {
      result = await _appController.GetExchangeRate("AUD", "AUD", 1);
    }

    [When(@"the user requests a conversion of two different set currencies")]
    public async void WhenTheUserRequestsAConversionOfTwoDifferentSetCurrencies()
    {
      result = await _appController.GetExchangeRate("USD", "AUD", 1);
    }

    [Then(@"the result should be (.*)")]
    public void ThenTheResultShouldBe(int p0)
    {
      Assert.AreEqual(p0, ((ExchangeRateResult)((JsonResult)result).Data).Value);
    }
  }
}
