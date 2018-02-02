using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Demo.Application.ApplicationServices;
using Demo.Application.QueryServices;
using Demo.Domain.Models.Accounts;
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
using LoginRequest = Demo.Tests.Models.LoginRequest;

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

        private IEnumerable<RegisterUserRequest> GetRegisterUserRequests(int count)
        {
            var ticks = DateTime.Now.Ticks;
            var step = 0;
            for (var i = 0; i < count; i++)
            {
                yield return new RegisterUserRequest
                {
                    UserName = $"Test{ticks}{step++}",
                    Password = "111111"
                };
            }
        }

        private async Task RegisterUserAsync(RegisterUserRequest request)
        {
            using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
            {
                var userAppService = scope.Resolve<UserAppService>();
                await userAppService.RegisterUserAsync(request);
            }
        }

        private Task RegisterUserAsync(int step)
        {
            return RegisterUserAsync(new RegisterUserRequest
            {
                UserName = $"Test{DateTime.Now.Ticks}{step}",
                Password = "111111"
            });
        }

        private async Task RegisterUserByHttpClientAsync(int step, ConcurrentBag<int> failedList)
        {
            try
            {
                using (var client = new TestsClient(new Uri("http://localhost:54395/")))
                {
                    client.HttpClient.Timeout = TimeSpan.FromSeconds(300);
                    var result = await client.DemoOperations.RegisterUserAsync(new Models.RegisterUserRequest
                    {
                        UserName = $"Test{DateTime.Now.Ticks}{step}",
                        Password = "111111"
                    });
                    if (!result?.Success ?? false)
                    {
                        failedList.Add(step);
                    }
                }
            }
            catch (Exception)
            {
                failedList.Add(step);
            }
        }

        private Task LoginUserAsync(LoginRequest request)
        {
            return LoginUserAsync(request.UserName, request.Password);
        }

        private async Task LoginUserAsync(string userName, string password)
        {
            using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
            {
                var userQueryService = scope.Resolve<UserQueryService>();
                await userQueryService.ValidateUserLoginAsync(userName, password);
            }
        }

        private async Task<LoginRequest[]> GetLoginUserRequests(int total)
        {
            // get users info
            using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
            {
                var repository = scope.Resolve<IDemoRepository>();
                return await repository.FindAll<Account>()
                                       .OrderBy(a => a.Id)
                                       .Take(total)
                                       .Select(a => new LoginRequest
                                       {
                                           UserName = a.UserName,
                                           Password = a.Password
                                       })
                                       .ToArrayAsync()
                                       .ConfigureAwait(false);
            }
        }

        private async Task LoginUserByHttpClientAsync(LoginRequest loginRequest, int step, ConcurrentBag<int> failedList)
        {
            try
            {
                using (var client = new TestsClient(new Uri("http://localhost:54395/")))
                {
                    client.HttpClient.Timeout = TimeSpan.FromSeconds(300);
                    var result = await client.DemoOperations.LoginAsync(loginRequest);
                    if (!result?.Success ?? false)
                    {
                        failedList.Add(step);
                    }
                }
            }
            catch (Exception)
            {
                failedList.Add(step);
            }
        }

        [Fact]
        public async Task ConcurrenceLoginTest()
        {
            var loginRequests = await GetLoginUserRequests(40000).ConfigureAwait(false);
            var tasks = new List<Task>();
            var taskQueue = new ConcurrentQueue<Task>();
            foreach (var request in loginRequests)
            {
                var task = new Task(async () =>
                {
                    await LoginUserAsync(request).ConfigureAwait(false);
                    if (taskQueue.TryDequeue(out var next))
                    {
                        next.Start();
                    }
                });
                tasks.Add(task);
                taskQueue.Enqueue(task);
            }
            // 预热
            await LoginUserAsync("string", "string").ConfigureAwait(false);
            await CodeTimer.TimeAsync(nameof(ConcurrenceLoginTest), 1, async () =>
            {
                // Start 100 tasks
                for (var i = 0; i < 10; i++)
                {
                    if (taskQueue.TryDequeue(out var task))
                    {
                        task.Start();
                    }
                }
                await Task.WhenAll(tasks);
            });
        }

        [Fact]
        public async Task ConcurrenceRegisterTest()
        {
            var registerUserRequets = GetRegisterUserRequests(40000).ToArray();
            var tasks = new List<Task>();
            var taskQueue = new ConcurrentQueue<Task>();
            foreach (var request in registerUserRequets)
            {
                var task = new Task(async () =>
                {
                    await RegisterUserAsync(request).ConfigureAwait(false);
                    if (taskQueue.TryDequeue(out var next))
                    {
                        next.Start();
                    }
                });
                tasks.Add(task);
                taskQueue.Enqueue(task);
            }
            // 预热
            await RegisterUserAsync(0).ConfigureAwait(false);
            await CodeTimer.TimeAsync(nameof(ConcurrenceRegisterTest), 1, async () =>
            {
                // Start 100 tasks
                for (var i = 0; i < 10; i++)
                {
                    if (taskQueue.TryDequeue(out var task))
                    {
                        task.Start();
                    }
                }
                await Task.WhenAll(tasks);
            });
        }

        [Fact]
        public async Task ConcurrenceLoginTestByHttpClient()
        {
            LoginRequest[] loginRequests;
            var total = 10000;
            // get users info
            using (var scope = IoCFactory.Instance.CurrentContainer.CreateChildContainer())
            {
                var repository = scope.Resolve<IDemoRepository>();
                loginRequests = await repository.FindAll<Account>()
                                                .OrderBy(a => a.Id)
                                                .Take(total)
                                                .Select(a => new LoginRequest
                                                {
                                                    UserName = a.UserName,
                                                    Password = a.Password
                                                })
                                                .ToArrayAsync()
                                                .ConfigureAwait(false);
            }

            await CodeTimer.TimeAsync(nameof(ConcurrenceLoginTestByHttpClient), 1, async () =>
            {
                var tasks = new List<Task>();
                var failedList = new ConcurrentBag<int>();
                for (var i = 0; i < loginRequests.Length; i++)
                {
                    tasks.Add(LoginUserByHttpClientAsync(loginRequests[i], i, failedList));
                }
                await Task.WhenAll(tasks);
                _output.WriteLine($"{failedList.Count} failed in {total}");
            });
        }

        [Fact]
        public Task ConcurrenceRegisterTestByHttpClient()
        {
            return CodeTimer.TimeAsync(nameof(ConcurrenceRegisterTestByHttpClient), 1, async () =>
            {
                var step = 0;
                var total = 10000;
                var tasks = new List<Task>();
                var failedList = new ConcurrentBag<int>();
                for (var i = 0; i < total; i++)
                {
                    tasks.Add(RegisterUserByHttpClientAsync(step++, failedList));
                }
                await Task.WhenAll(tasks);
                _output.WriteLine($"{failedList.Count} failed in {total}");
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
                        await userAppService.RegisterUsersAsync(GetRegisterUserRequests(10));
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