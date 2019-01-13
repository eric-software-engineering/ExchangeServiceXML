using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ExchangeRateXML.Models.RedisCache
{
  public class AzureRedisControllerCache : IApiControllerCache
  {
    private IDatabase cache;

    private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
      return ConnectionMultiplexer.Connect("ExchangeRateXML.redis.cache.windows.net:6380,password=N7PY1K8Bc+w4+RKlg33Sps+ooil7b8dOUbBD960+kvM=,ssl=True,abortConnect=False");
    });

    // C#6 Body expression
    public static ConnectionMultiplexer Connection => LazyConnection.Value;

    public async Task<object> Get(string CacheKey)
    {
      if (cache == null)
        cache = Connection.GetDatabase();
      return await Task.Run(() => cache.Get(CacheKey)).ConfigureAwait(false);
    }

    public async Task Put(object value, string CacheKey)
    {
      if (cache == null)
        cache = Connection.GetDatabase();
      await Task.Run(() => cache.Set(CacheKey, value, new TimeSpan(1, 0, 0, 0))).ConfigureAwait(false);
    }
  }
}