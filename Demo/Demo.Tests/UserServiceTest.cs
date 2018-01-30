using IFramework.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Demo.Application.ApplicationServices;
using Demo.Domain.Repositories;
using Demo.Domain.Services;
using Demo.DTO.RequestModels.Accounts;
using Demo.Infrastructure;
using Demo.Persistence;
using IFramework.IoC;
using Xunit;
using IFramework.FoundatioLock.Config;
using IFramework.UnitOfWork;
using IFramework.EntityFramework.Config;

namespace Demo.Tests
{
    public class UserServiceTest
    {
        public const string App = "Test";
        public UserServiceTest()
        {
            Configuration.Instance
                         .UseAutofacContainer()
                         .RegisterAssemblyTypes("Demo.Application", "Demo.Domain")
                         .RegisterCommonComponents()
                         .UseLog4Net(App)
                         .UseJsonNet()
                         .UseFoundatioLockInMemory();

            var container = IoCFactory.Instance.CurrentContainer;
            RegisterTypes(container, Lifetime.Hierarchical);
        }

        private void RegisterTypes(IContainer container, Lifetime lifetime)
        {
            Configuration.Instance
                         .RegisterEntityFrameworkComponents(container, lifetime);

            container.RegisterType<DemoDbContext, DemoDbContext>(lifetime);
            container.RegisterType<IDemoRepository, DemoRepository>(lifetime);
            container.RegisterType<IAppUnitOfWork, AppUnitOfWork>(lifetime);
            container.RegisterType<IEncryptService, EncryptService>(lifetime);
        }

        [Fact]
        public async Task TestRegisterUserAsync()
        {
            for (int i = 0; i < 1000; i++)
            {
                using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
                {
                    var userAppService = scope.Resolve<UserAppService>();
                    await userAppService.RegisterUsersAsync(GetRegisterUsersRequest());

                }
            }
        }
        private IEnumerable<RegisterUserRequest> GetRegisterUsersRequest()
        {
            long ticks = DateTime.Now.Ticks;
            int Step = 0;
            for (int i = 0; i < 10000; i++)
            {
                yield return new RegisterUserRequest
                {
                    UserName = $"Test_{ticks}{Step++}",
                    Password = "111111"
                };
            }
        }
    }
}
