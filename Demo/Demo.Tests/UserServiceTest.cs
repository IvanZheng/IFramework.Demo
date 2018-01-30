using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Application.ApplicationServices;
using Demo.Application.QueryServices;
using Demo.Domain.Repositories;
using Demo.Domain.Services;
using Demo.DTO.RequestModels.Accounts;
using Demo.Infrastructure;
using Demo.Persistence;
using IFramework.Config;
using IFramework.EntityFramework.Config;
using IFramework.FoundatioLock.Config;
using IFramework.IoC;
using IFramework.UnitOfWork;
using Xunit;

namespace Demo.Tests
{
    public class UserServiceTest
    {
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

        public const string App = "Test";

        private void RegisterTypes(IContainer container, Lifetime lifetime)
        {
            Configuration.Instance
                         .RegisterEntityFrameworkComponents(container, lifetime);

            container.RegisterType<DemoDbContext, DemoDbContext>(lifetime);
            container.RegisterType<IDemoRepository, DemoRepository>(lifetime);
            container.RegisterType<IAppUnitOfWork, AppUnitOfWork>(lifetime);
            container.RegisterType<IEncryptService, EncryptService>(lifetime);
        }

        private IEnumerable<RegisterUserRequest> GetRegisterUsersRequest()
        {
            var ticks = DateTime.Now.Ticks;
            var step = 0;
            for (var i = 0; i < 1; i++)
            {
                yield return new RegisterUserRequest
                {
                    UserName = $"Test_{ticks}{step++}",
                    Password = "111111"
                };
            }
        }

        [Fact]
        public async Task TestLoginUserAsync()
        {
            using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
            {
                var userQueryService = scope.Resolve<UserQueryService>();
                await userQueryService.ValidateUserLoginAsync("string", "BCC613D9C97CA9AA");
            }
        }

        [Fact]
        public async Task TestRegisterUserAsync()
        {
            using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
            {
                var userAppService = scope.Resolve<UserAppService>();
                await userAppService.RegisterUserAsync(new RegisterUserRequest
                {
                    UserName = $"Test_{DateTime.Now.Ticks}",
                    Password = "111111"
                });
            }
        }

        [Fact]
        public async Task TestRegisterUsersAsync()
        {
            for (var i = 0; i < 1; i++)
            {
                using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
                {
                    var userAppService = scope.Resolve<UserAppService>();
                    await userAppService.RegisterUsersAsync(GetRegisterUsersRequest());
                }
            }
        }
    }
}