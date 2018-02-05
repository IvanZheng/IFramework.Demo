using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.Domain;
using Demo.Domain.Models.Accounts;
using Demo.Domain.Repositories;
using Demo.Domain.Services;
using Demo.DTO.Enums;
using Demo.Persistence;
using IFramework.Exceptions;
using VM = Demo.DTO.ViewModels.Users;

namespace Demo.Application.QueryServices
{
    public class UserQueryService : QueryServiceBase
    {
        private readonly DemoDbContext _dbContext;
        private readonly IEncryptService _encryptService;

        public UserQueryService(IDemoRepository repository,
                                IEncryptService encryptService,
                                DemoDbContext dbContext)
            : base(repository)
        {
            _encryptService = encryptService;
            _dbContext = dbContext;
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
            //return await FindUserByInternalAccountSqlAsync(accountSpec).ConfigureAwait(false);


            //return await _dbContext.Database
            //                       .SqlQuery<VM.ApplicationUser>(sql,
            //                                               new SqlParameter(nameof(accountSpec.UserName), accountSpec.UserName),
            //                                               new SqlParameter(nameof(accountSpec.Password), accountSpec.Password))
            //                 .FirstOrDefaultAsync()
            //                 .ConfigureAwait(false);

            //var account =  Repository.Find(accountSpec);
            //return new VM.ApplicationUser(account.UserId, account.UserName, account.Id, account.AccountType, true, null);


            return await Repository.FindAll(accountSpec)
                                   .Select(a => new VM.ApplicationUser
                                   {
                                       Id = a.UserId,
                                       AccountId = a.Id,
                                       UserName = a.UserName,
                                       UserToken = a.UserName,
                                       Enabled = true,
                                       AccountType = a.AccountType
                                   })
                                   .FirstOrDefaultAsync()
                                   .ConfigureAwait(false);


            //var userQuery = from account in Repository.FindAll(accountSpec)
            //                join u in Repository.FindAll<User>()
            //                    on account.UserId equals u.Id
            //                select new { u, account };
            //var userAccount = await userQuery.FirstOrDefaultAsync()
            //                                 .ConfigureAwait(false);
            //return userAccount?.u.ToVm(userAccount.account);
        }

        private static async Task<VM.ApplicationUser> FindUserByInternalAccountSqlAsync(AccountSpec accountSpec)
        {
            var sql = $"select top 1 * from Accounts where [AccountType] = 0 and [UserName] = @username and [Password] = @password ";

            using (var conn = new SqlConnection("Server=(localdb)\\projects;Database=DemoDb;Integrated Security=true;"))
            {
                await conn.OpenAsync()
                          .ConfigureAwait(false);
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddRange(new[]
                {
                    new SqlParameter(nameof(accountSpec.UserName), accountSpec.UserName),
                    new SqlParameter(nameof(accountSpec.Password), accountSpec.Password)
                });
                var result = await cmd.ExecuteReaderAsync(CancellationToken.None)
                                      .ConfigureAwait(false);
                if (result.HasRows && await result.ReadAsync().ConfigureAwait(false))
                {
                    return new VM.ApplicationUser((string) result["UserId"],
                                                  (string) result["UserName"],
                                                  (string) result["Id"],
                                                  AccountType.Internal,
                                                  true,
                                                  (string) result["UserName"]);
                }
                return null;
            }
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

        public async Task<Tuple<VM.ApplicationUser, DomainException>> ValidateUserLoginWithoutExceptionAsync(string userName, string password)
        {
            DomainException exception = null;
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            }
            var user = await FindUserByInternalAccountAsync(userName, password).ConfigureAwait(false);
            if (user == null)
            {
                exception = new DomainException(Error.WrongUserNameOrPassword);
            }
            else
            {
                if (!user.Enabled)
                {
                    exception = new DomainException(Error.UserUnavailable);
                }
            }
            return new Tuple<VM.ApplicationUser, DomainException>(user, exception);
        }
    }
}