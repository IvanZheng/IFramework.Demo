using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Demo.DTO.Enums;
using IFramework.Specifications;

namespace Demo.Domain.Models.Accounts
{
    public class AccountSpec: Specification<Account>
    {
        public string UserName { get; protected set; }
        public string Password { get; protected set; }

        public string AccountId { get; protected set; }

        public AccountSpec(string accountId)
        {
            AccountId = accountId;
        }

        public AccountSpec(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public override Expression<Func<Account, bool>> GetExpression()
        {
            if (string.IsNullOrWhiteSpace(AccountId))
            {
                return a => a.AccountType == AccountType.Internal && a.UserName == UserName && a.Password == Password;
            }
            else
            {
                return a => a.Id == AccountId;
            }
        }
    }
}
