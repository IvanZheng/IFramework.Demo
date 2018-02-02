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
    public class AccountExistsSpec:Specification<Account>
    {
        private readonly AccountType _accountType;
        private readonly string _username;

        public AccountExistsSpec(AccountType accountType, string username)
        {
            _accountType = accountType;
            _username = username;
        }


        public override Expression<Func<Account, bool>> GetExpression()
        {
            return a => a.AccountType == _accountType && a.UserName == _username;
        }
    }
}
