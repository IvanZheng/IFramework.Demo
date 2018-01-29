using System.Threading.Tasks;
using IFramework.EntityFramework.Repositories;
using IFramework.Exceptions;
using IFramework.IoC;
using IFramework.UnitOfWork;
using Demo.Domain.Models;
using Demo.Domain.Repositories;
using Demo.Domain;
using Demo.Domain.Models.Accounts;
using Demo.Domain.Models.Users;

namespace Demo.Persistence
{
    public class DemoRepository : DomainRepository, IDemoRepository
    {
        public DemoRepository(DemoDbContext dbContext, IAppUnitOfWork uow, IContainer container)
            : base(dbContext, uow, container) { }

        

        public async Task<Account> GetAccountAsync(string accountId, bool throwExceptionIfNotExists = false)
        {
            var account = await GetByKeyAsync<Account>(accountId).ConfigureAwait(false);
            if (account == null && throwExceptionIfNotExists)
            {
                throw new DomainException(Error.AccountNotExists, new object[] { accountId });
            }
            return account;
        }

        public async Task<User> GetUserAsync(string userId, bool throwExceptionIfNotExists = false)
        {
            var user = await GetByKeyAsync<User>(userId).ConfigureAwait(false);
            if (user == null && throwExceptionIfNotExists)
            {
                throw new DomainException(Error.UserNotExists, new object[] { userId });
            }
            return user;
        }
    }
}