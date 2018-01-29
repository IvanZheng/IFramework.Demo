using System.Linq;
using System.Threading.Tasks;
using Demo.Application.Adapters;
using Demo.Domain;
using Demo.Domain.Models.Accounts;
using Demo.Domain.Models.Users;
using Demo.Domain.Repositories;
using Demo.DTO.Enums;
using IFramework.EntityFramework.Repositories;
using IFramework.Exceptions;
using VM = Demo.DTO.ViewModels.Users;

namespace Demo.Application.QueryServices
{
    public class UserQueryService : QueryServiceBase
    {
        public UserQueryService(IDemoRepository repository)
            : base(repository) { }

        public Task<VM.ApplicationUser> FindUserByInternalAccountAsync(string userName, string password)
        {
            return FindUserByInternalAccountAsync(new AccountSpec(userName, password));
        }

        public Task<VM.ApplicationUser> FindUserByAccountIdAsync(string accountId)
        {
            return FindUserByInternalAccountAsync(new AccountSpec(accountId));
        }

        private async Task<VM.ApplicationUser> FindUserByInternalAccountAsync(AccountSpec accountSpec)
        {
            var userQuery = from account in Repository.FindAll(accountSpec)
                            join u in Repository.FindAll<User>()
                                on account.UserId equals u.Id
                            select new { u, account };
            var userAccount = await userQuery.FirstOrDefaultAsync()
                                             .ConfigureAwait(false);
            return userAccount?.u.ToVm(userAccount.account);
        }

        public async Task<VM.ApplicationUser> ValidateUserLoginAsync(string userName, string password)
        {
            var user = await FindUserByInternalAccountAsync(userName, password).ConfigureAwait(false);
            if (user == null)
            {
                throw new DomainException(Error.WrongUserNameOrPassword);
            }
            if (!user.Enabled)
            {
                throw new DomainException(Error.UserUnavailable);
            }
            return user;
        }
    }
}