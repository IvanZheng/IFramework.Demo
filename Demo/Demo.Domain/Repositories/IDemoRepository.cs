using System.Threading.Tasks;
using Demo.Domain.Models.Accounts;
using Demo.Domain.Models.Users;
using IFramework.Repositories;

namespace Demo.Domain.Repositories
{
    public interface IDemoRepository : IDomainRepository
    {
        Task<Account> GetAccountAsync(string accountId, bool throwExceptionIfNotExists = false);
        Task<User> GetUserAsync(string userId, bool throwExceptionIfNotExists = false);
        Task<long> GetNextSequenceAsync(string dbsequence);
    }
}