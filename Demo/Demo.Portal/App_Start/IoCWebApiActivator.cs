using IFramework.Config;
using IFramework.IoC;
using IFramework.IoC.WebApi;
using System.Web.Http;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Demo.Portal.App_Start.IoCWebApiActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Demo.Portal.App_Start.IoCWebApiActivator), "Shutdown")]

namespace Demo.Portal.App_Start
{
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary>
    public static class IoCWebApiActivator
    {
        static IContainer _container;
    
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
            _container = IoCConfig.GetConfiguredContainer();
            var resolver = new HierarchicalDependencyResolver(_container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            _container?.Dispose();
        }
    }
}
