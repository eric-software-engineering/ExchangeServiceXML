using System.Threading.Tasks;

namespace ExchangeRateXML.Models.RedisCache
{
  public interface IApiControllerCache
  {
    Task<object> Get(string CacheKey);
    Task Put(object value, string CacheKey);
  }
}