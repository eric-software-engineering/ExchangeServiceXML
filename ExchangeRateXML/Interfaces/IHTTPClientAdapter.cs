using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeRateXML.Interfaces
{
  // SOLID Principle: [I]nterface Segregation
  public interface IHTTPClientAdapter : IDisposable
  {
    Task<HttpResponseMessage> GetAsync();
  }
}
