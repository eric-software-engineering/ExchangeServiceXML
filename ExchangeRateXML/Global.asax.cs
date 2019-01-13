using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ExchangeRateXML.Interfaces;
using ExchangeRateXML.Models;
using ExchangeRateXML.Models.RedisCache;

namespace ExchangeRateXML
{
  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      // Create the container as usual.
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

      // Register your types, for instance:
      container.Register<IRepository<AzureRedisControllerCache>, CacheAccess>(Lifestyle.Scoped);
      container.Register<IHTTPClientAdapter, HTTPClientAdapter>(Lifestyle.Scoped);
      container.Register<IRepository<IHTTPClientAdapter>, XmlClientAdapter>(Lifestyle.Scoped);

      // This is an extension method from the integration package.
      container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

      container.Verify();

      DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
    }
  }
}
