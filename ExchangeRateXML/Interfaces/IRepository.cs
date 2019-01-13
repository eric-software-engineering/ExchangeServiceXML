using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeRateXML.Models;

namespace ExchangeRateXML.Interfaces
{
  // SOLID Principle: [I]nterface Segregation
  public interface IRepository<T>
  {
    Task InsertData(List<ExchangeRate> rates);
    Task<ExchangeRate> GetData(string baseCurrency, string targetCurrency);
    Task<List<ExchangeRate>> GetAllData();
    Task<List<Currency>> GetCurrencies();
    Task<DateTime?> GetTimeStamp();
  }
}
