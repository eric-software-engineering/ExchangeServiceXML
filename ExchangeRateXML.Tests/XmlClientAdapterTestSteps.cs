using ExchangeRateXML.Interfaces;
using ExchangeRateXML.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace ExchangeRateXML.Tests
{
  [Binding]
  public sealed class XmlClientAdapterTestSteps
  {
    public XmlClientAdapter CurrentXmlClientAdapter;
    private Mock<IHTTPClientAdapter> _httpClientAdapterMock;
    private List<ExchangeRate> _result;

    [Given(@"there is an error when connecting to the XML file")]
    public void GivenThereIsAnErrorWhenConnectingToTheXMLFile()
    {
      _httpClientAdapterMock = new Mock<IHTTPClientAdapter>();
      _httpClientAdapterMock.Setup(x => x.GetAsync()).Throws(new IOException());
      CurrentXmlClientAdapter = new XmlClientAdapter(_httpClientAdapterMock.Object);
    }

    [Then(@"there is an error when running the adapter")]
    [ExpectedException(typeof(IOException))]
    public async void ThenThereIsAnErrorWhenRunningTheAdapter()
    {
      _result = await CurrentXmlClientAdapter.GetAllData();
    }

    [Given(@"The data given is correct")]
    public void GivenTheDataGivenIsCorrect()
    {
      var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
      resp.Content = new StringContent(File.ReadAllText(Directory.GetCurrentDirectory() + @"\CurrencyMocks\Currencies.xml"));

      _httpClientAdapterMock = new Mock<IHTTPClientAdapter>();
      _httpClientAdapterMock.Setup(x => x.GetAsync()).Returns(Task.FromResult<HttpResponseMessage>(resp));
      CurrentXmlClientAdapter = new XmlClientAdapter(_httpClientAdapterMock.Object);
    }

    [When(@"the API is called")]
    public async void WhenTheAPIIsCalled()
    {
      _result = await CurrentXmlClientAdapter.GetAllData().ConfigureAwait(false);
    }

    [Then(@"the values returned by the adapter are correct")]
    public void ThenTheValuesReturnedByTheAdapterAreCorrect()
    {
      Assert.IsTrue(_result.Count == 9);
      Assert.IsTrue(_result.FirstOrDefault(x => x.baseCurrency == "USD" && x.targetCurrency == "AUD")?.exchangeRate == 2);
      Assert.IsTrue(_result.FirstOrDefault(x => x.baseCurrency == "USD" && x.targetCurrency == "EUR")?.exchangeRate == 3);
      Assert.IsTrue(_result.FirstOrDefault(x => x.baseCurrency == "AUD" && x.targetCurrency == "EUR")?.exchangeRate == (decimal?)1.5);
    }
  }
}
