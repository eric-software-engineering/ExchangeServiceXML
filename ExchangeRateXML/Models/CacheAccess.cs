using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRateXML.Interfaces;
using ExchangeRateXML.Models.RedisCache;

namespace ExchangeRateXML.Models
{
  public class CacheAccess : IRepository<AzureRedisControllerCache>
  {
    public async Task<ExchangeRate> GetData(string baseCurrency, string targetCurrency)
    {
      return (await GetAllData().ConfigureAwait(false)).FirstOrDefault(x => x.baseCurrency == baseCurrency && x.targetCurrency == targetCurrency);
    }

    public async Task InsertData(List<ExchangeRate> rates)
    {
      var cacheRep = new AzureRedisControllerCache();
      await cacheRep.Put(rates, "ExchangeRate").ConfigureAwait(false);
    }

    public async Task<List<ExchangeRate>> GetAllData()
    {
      var cacheRep = new AzureRedisControllerCache();
      List<ExchangeRate> result = null;

      // C#7 pattern matching
      if (await cacheRep.Get("ExchangeRate").ConfigureAwait(false) is List<ExchangeRate> cache)
        result = cache;

      return result;
    }

    public async Task<List<Currency>> GetCurrencies()
    {
      // No hard-coded list of currencies. If more currencies are included to the XML file, the code will pick them up automatically
      return (await GetAllData().ConfigureAwait(false)).GroupBy(x => x.baseCurrency).Select(node => new Currency() { Code = node.First().baseCurrency }).ToList();
    }

    public async Task<DateTime?> GetTimeStamp()
    {
      return (await GetAllData().ConfigureAwait(false))?.FirstOrDefault()?.timestamp;
    }
  }
}
