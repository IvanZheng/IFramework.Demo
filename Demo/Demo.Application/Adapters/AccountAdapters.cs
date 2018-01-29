using Demo.Domain.Models.Accounts;
using Demo.Domain.Models.Users;
using Demo.DTO.ViewModels.Users;

namespace Demo.Application.Adapters
{
    public static class AccountAdapters
    {
        public static ApplicationUser ToVm(this User user, Account account)
        {
            return new ApplicationUser(user.Id,
                                       user.Name,
                                       account.Id,
                                       account.AccountType,
                                       user.Enabled,
                                       account.UserName);
        }
    }
}