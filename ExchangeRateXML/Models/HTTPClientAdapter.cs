using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRateXML.Interfaces;

namespace ExchangeRateXML.Models
{
  // SOLID Principle: [S]ingle Responsibility
  public class HTTPClientAdapter : IHTTPClientAdapter
  {
    private readonly HttpClient _httpClient = new HttpClient();

    public async Task<HttpResponseMessage> GetAsync()
    {
      // File stored in the Azure File Storage: high performance, supports load balancing
      // SAS Token to access the file for improved security
      var SAS_Key = "sv=2018-03-28&ss=bfqt&srt=sco&sp=rwdlacup&se=2019-10-11T19:19:07Z&st=2019-01-11T11:19:07Z&spr=https&sig=E8HB3buff6m25ef%2F0GHwynfCpOTUI5r993vBJaUZJ6g%3D";
      
      // C# 6: String interpolation
      return await _httpClient.GetAsync($"https://exchangeratexml.file.core.windows.net/exchangerate/Currencies.xml?{SAS_Key}").ConfigureAwait(false);
    }

    public void Dispose()
    {
      _httpClient.Dispose();
    }
  }
}