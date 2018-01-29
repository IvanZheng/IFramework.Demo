using System.Linq;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using IFramework.Autofac;
using IFramework.IoC;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Demo.Portal.App_Start.AutofacMvcActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Demo.Portal.App_Start.AutofacMvcActivator), "Shutdown")]

namespace Demo.Portal.App_Start
{
    /// <summary>Provides the bootstrapping for integrating Autofac with ASP.NET MVC.</summary>
    public static class AutofacMvcActivator
    {
        static IContainer _container;
        
        /// <summary>Integrates Autofac when the application starts.</summary>
        public static void Start() 
        {
            _container = IoCConfig.GetMvcConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new AutofacFilterProvider());

            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container.GetAutofacContainer()));
        }

        /// <summary>Disposes the Autofac container when the application is shut down.</summary>
        public static void Shutdown()
        {
            _container?.Dispose();
        }
    }
}