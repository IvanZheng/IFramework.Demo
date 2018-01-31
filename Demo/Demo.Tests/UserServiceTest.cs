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
using Xunit.Abstractions;

namespace Demo.Tests
{
    public class UserServiceTest
    {
        public UserServiceTest(ITestOutputHelper output)
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
            _output = output;
            CodeTimer.Initialize(output);
        }

        private readonly ITestOutputHelper _output;

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
            for (var i = 0; i < 10; i++)
            {
                yield return new RegisterUserRequest
                {
                    UserName = $"Test{ticks}{step++}",
                    Password = "111111"
                };
            }
        }

        private async Task RegisterUserAsync(int step)
        {
            using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
            {
                var userAppService = scope.Resolve<UserAppService>();
                await userAppService.RegisterUserAsync(new RegisterUserRequest
                {
                    UserName = $"Test{DateTime.Now.Ticks}{step}",
                    Password = "111111"
                });
            }
        }

        private async Task RegisterUserByHttpClientAsync(int step)
        {
            using (var client = new TestsClient(new Uri("http://localhost:54395/")))
            {
                await client.DemoOperations.RegisterUserAsync(new Models.RegisterUserRequest
                {
                    UserName = $"Test{DateTime.Now.Ticks}{step}",
                    Password = "111111"
                });
            }
        }

        private async Task LoginUserAsync(string userName, string password)
        {
            using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
            {
                var userQueryService = scope.Resolve<UserQueryService>();
                await userQueryService.ValidateUserLoginAsync(userName, password);
            }
        }

        [Fact]
        public Task ConcurrenceLoginTest()
        {
            return CodeTimer.TimeAsync(nameof(ConcurrenceLoginTest), 1, async () =>
            {
                var tasks = new List<Task>();
                for (var i = 0; i < 10000; i++)
                {
                    tasks.Add(LoginUserAsync("string", "string"));
                }
                await Task.WhenAll(tasks);
            });
        }

        [Fact]
        public Task ConcurrenceRegisterTest()
        {
            return CodeTimer.TimeAsync(nameof(ConcurrenceRegisterTest), 1, async () =>
            {
                var step = 0;
                var tasks = new List<Task>();
                for (var i = 0; i < 10000; i++)
                {
                    tasks.Add(RegisterUserAsync(step++));
                }
                await Task.WhenAll(tasks);
            });
        }

        [Fact]
        public Task ConcurrenceRegisterTestByHttpClient()
        {
            return CodeTimer.TimeAsync(nameof(ConcurrenceRegisterTestByHttpClient), 1, async () =>
            {
                var step = 0;
                var tasks = new List<Task>();
                for (var i = 0; i < 10000; i++)
                {
                    tasks.Add(RegisterUserByHttpClientAsync(step++));
                }
                await Task.WhenAll(tasks);
            });
        }

        [Fact]
        public Task TestBatchRegisterUsersAsync()
        {
            return CodeTimer.TimeAsync(nameof(TestBatchRegisterUsersAsync), 1, async () =>
            {
                for (var i = 0; i < 10; i++)
                {
                    using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
                    {
                        var userAppService = scope.Resolve<UserAppService>();
                        await userAppService.RegisterUsersAsync(GetRegisterUsersRequest());
                    }
                }
            });
        }

        [Fact]
        public Task TestLoginUserAsync()
        {
            return CodeTimer.TimeAsync(nameof(TestLoginUserAsync),
                                       10000,
                                       () => LoginUserAsync("string", "string"));
        }

        [Fact]
        public Task TestRegisterUserAsync()
        {
            var step = 0;
            return CodeTimer.TimeAsync(nameof(TestRegisterUserAsync),
                                       10000,
                                       () => RegisterUserAsync(step++));
        }
    }
}