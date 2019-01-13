using ExchangeRateXML.ExtensionMethods;
using ExchangeRateXML.Models.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExchangeRateXML.Interfaces;
using ExchangeRateXML.Models.RedisCache;

namespace ExchangeRateXML.Controllers
{
  public class HomeController : Controller
  {
    // Cannot use "lock" with awaitable tasks. Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
    private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

    private readonly IRepository<AzureRedisControllerCache> _cacheRepository;
    private readonly IRepository<IHTTPClientAdapter> _apiClient;

    // SOLID PRINCIPLE: [D]ependency Inversion
    // C#7 Expression Bodied Constructor
    public HomeController(IRepository<AzureRedisControllerCache> cacheRepository, IRepository<IHTTPClientAdapter> apiClient) => (_cacheRepository, _apiClient) = (cacheRepository, apiClient);

    public async Task<ActionResult> Index()
    {
      await RepositoryManager();
      return View(await _cacheRepository.GetCurrencies());
    }

    [Route("api/{from}/{to}/{amount}")]
    public async Task<ActionResult> GetExchangeRate(string from, string to, decimal amount)
    {
      if (from == to)
        return Json(new ExchangeRateResult() { Status = Status.Success, Value = amount }, JsonRequestBehavior.AllowGet);

      await RepositoryManager();
      var result = await _cacheRepository.GetData(from.ToUpper(), to.ToUpper());

      if (result == null)
        return Json(new ExchangeRateResult() { Status = Status.Failure, Value = 0 }, JsonRequestBehavior.AllowGet);

      return Json(new ExchangeRateResult() { Status = Status.Success, Value = result.exchangeRate * amount }, JsonRequestBehavior.AllowGet);
    }

    // This method makes sure that the cache is populated before being accessed.
    // It also ensures that only one thread accesses the XML file at any given time
    private async Task RepositoryManager()
    {
      try
      {
        // Load-balancing issue with cache: if the cache is on each server, then they all have their own independent lifecycle
        // Solution: use the Azure Redis, a shared cache with high performance at low cost
        var result = await _cacheRepository.GetTimeStamp().ConfigureAwait(false);

        // The cache is full and up-to-date (<10 minutes): no need to refresh it
        if (result != null && !(result < DateTime.Now.AddMinutes(-10))) return;

        // Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released.
        await SemaphoreSlim.WaitAsync().ConfigureAwait(false);

        // Maybe another thread fetched the cache in the meantime. Returning it.
        result = await _cacheRepository.GetTimeStamp().ConfigureAwait(false);
        if (result == null || result < DateTime.Now.AddMinutes(-10))
          // This is the first thread to find an empty cache. Refilling it.
          await _cacheRepository.InsertData(await _apiClient.GetAllData().ConfigureAwait(false)).ConfigureAwait(false);
      }
      catch (Exception e)
      {
        e.TraceException();
        // Something didn't work. Logging the exception and showing the error in the interface 
        throw;
      }
      finally
      {
        // When the task is ready, release the semaphore. 
        if (SemaphoreSlim.CurrentCount == 0)
          SemaphoreSlim.Release();
      }
    }

    public ActionResult Contact()
    {
      return View();
    }
  }
}