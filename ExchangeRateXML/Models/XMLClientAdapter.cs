using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ExchangeRateXML.Interfaces;

namespace ExchangeRateXML.Models
{
  // SOLID Principle: [S]ingle Responsibility
  public class XmlClientAdapter : IRepository<IHTTPClientAdapter>
  {
    // Adapter Design Pattern
    private readonly IHTTPClientAdapter _httpClient;

    // C#7 Expression Bodied Constructor
    public XmlClientAdapter(IHTTPClientAdapter httpClient) => (_httpClient) = (httpClient);

    public async Task<List<ExchangeRate>> GetAllData()
    {
      var response = await _httpClient.GetAsync().ConfigureAwait(false);

      // Defensive programming
      response.EnsureSuccessStatusCode();

      // Optimization using asynchronous programming: while the IO operation is executed, we can free the thread
      var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

      var doc = new XmlDocument();
      doc.LoadXml(responseBody);

      // XPath
      var nodes = doc.SelectNodes("//Currency");

      // C# 6: Null-conditional operator
      if (nodes?.Count == 0) throw new IOException("Invalid XML File");

      var baseConversion = nodes?.Cast<XmlNode>()
        .Select(node => new ExchangeRate()
        {
          baseCurrency = "USD",
          targetCurrency = node.SelectSingleNode("Code")?.InnerText,
          exchangeRate = Convert.ToDecimal(node.SelectSingleNode("ExchangeRate")?.InnerText)
        }).ToList();

      var currenciesToConvert = baseConversion.Where(x => x.targetCurrency != "USD").ToList();
      var result = new List<ExchangeRate>();
      result.AddRange(baseConversion);

      foreach (var item in currenciesToConvert)
      {
        foreach (var item2 in baseConversion)
        {
          result.Add(new ExchangeRate()
          {
            baseCurrency = item.targetCurrency,
            targetCurrency = item2.targetCurrency,
            exchangeRate = Math.Round(item2.exchangeRate / item.exchangeRate, 2)
          });
        }
      }

      return result;
    }

    public Task<ExchangeRate> GetData(string baseCurrency, string targetCurrency)
    {
      throw new NotImplementedException();
    }

    public Task InsertData(List<ExchangeRate> rates)
    {
      throw new NotImplementedException();
    }

    public Task<List<Currency>> GetCurrencies()
    {
      throw new NotImplementedException();
    }

    public Task<DateTime?> GetTimeStamp()
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      _httpClient.Dispose();
    }
  }
}