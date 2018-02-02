using System;
using System.Linq;
using System.Threading.Tasks;
using Demo.Application.Adapters;
using Demo.Domain;
using Demo.Domain.Models.Accounts;
using Demo.Domain.Models.Users;
using Demo.Domain.Repositories;
using Demo.Domain.Services;
using Demo.DTO.Enums;
using IFramework.EntityFramework.Repositories;
using IFramework.Exceptions;
using VM = Demo.DTO.ViewModels.Users;

namespace Demo.Application.QueryServices
{
    public class UserQueryService : QueryServiceBase
    {
        private readonly IEncryptService _encryptService;

        public UserQueryService(IDemoRepository repository,
                                IEncryptService encryptService)
            : base(repository)
        {
            _encryptService = encryptService;
        }

        public Task<VM.ApplicationUser> FindUserByInternalAccountAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            }
            password = _encryptService.EncryptPassword(password);
            return FindUserByInternalAccountAsync(new AccountSpec(userName, password));
        }

        public Task<VM.ApplicationUser> FindUserByAccountIdAsync(string accountId)
        {
            return FindUserByInternalAccountAsync(new AccountSpec(accountId));
        }

        private async Task<VM.ApplicationUser> FindUserByInternalAccountAsync(AccountSpec accountSpec)
        {
            var account = await Repository.FindAsync(accountSpec)
                                          .ConfigureAwait(false);
            return new VM.ApplicationUser(account.UserId, account.UserName, account.Id, account.AccountType, true, null);
            //var userQuery = from account in Repository.FindAll(accountSpec)
            //                join u in Repository.FindAll<User>()
            //                    on account.UserId equals u.Id
            //                select new { u, account };
            //var userAccount = await userQuery.FirstOrDefaultAsync()
            //                                 .ConfigureAwait(false);
            //return userAccount?.u.ToVm(userAccount.account);
        }

        public async Task<VM.ApplicationUser> ValidateUserLoginAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            }
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