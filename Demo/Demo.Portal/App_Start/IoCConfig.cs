using System;
using Demo.Domain.Repositories;
using Demo.Domain.Services;
using Demo.Infrastructure;
using Demo.Persistence;
using IFramework.Config;
using IFramework.EntityFramework.Config;
using IFramework.FoundatioLock.Config;
using IFramework.IoC;
using IFramework.UnitOfWork;

namespace Demo.Portal.App_Start
{
    /// <summary>
    /// Specifies the IoC configuration for the main container.
    /// </summary>
    public class IoCConfig
    {
        public static readonly string App = Configuration.GetAppConfig("DefaultApp");
        #region IoC Container
        private static Lazy<IContainer> container = new Lazy<IContainer>(() =>
        {
            Configuration.Instance
                         .UseAutofacContainer()
                         .RegisterCommonComponents()
                         .UseLog4Net(App)
                         .UseJsonNet()
                         .UseFoundatioLockInMemory();
            
            var container = IoCFactory.Instance.CurrentContainer;
            RegisterTypes(container, Lifetime.Hierarchical);
            return container;
        });

        /// <summary>
        /// Gets the configured IoC container.
        /// </summary>
        public static IContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion
        
        #region Mvc Container
        private static Lazy<IContainer> mvcContainer = new Lazy<IContainer>(() =>
        {
            var container = GetConfiguredContainer().CreateChildContainer();
            RegisterTypes(container, Lifetime.PerRequest);
            return container;
        });
        
        /// <summary>
        /// Gets the configured Mvc container.
        /// </summary>
        public static IContainer GetMvcConfiguredContainer()
        {
           return mvcContainer.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the IoC container.</summary>
        /// <param name="container">The IoC container to configure.</param>
        /// <param name="lifetime"></param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as IoC allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IContainer container, Lifetime lifetime)
        {
            Configuration.Instance
                         .RegisterEntityFrameworkComponents(container, lifetime);

            container.RegisterType<DemoDbContext, DemoDbContext>(lifetime);
            container.RegisterType<IDemoRepository, DemoRepository>(lifetime);
            container.RegisterType<IAppUnitOfWork, AppUnitOfWork>(lifetime);
            container.RegisterType<IEncryptService, EncryptService>(lifetime);
        }
    }
}
