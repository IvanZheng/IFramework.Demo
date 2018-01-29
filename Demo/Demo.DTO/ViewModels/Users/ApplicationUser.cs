using System;
using Demo.DTO.Enums;
using Microsoft.AspNet.Identity;

namespace Demo.DTO.ViewModels.Users
{
    /// <summary>
    ///     用户账号信息
    /// </summary>
    public class ApplicationUser : IUser
    {
        public ApplicationUser(string id, string userName, string accountId, AccountType accountType, bool enabled, string userToken)
        {
            AccountType = accountType;
            Enabled = enabled;
            UserToken = userToken;
            Id = id ?? throw new ArgumentNullException(nameof(id));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            AccountId = accountId;
        }

        /// <summary>
        ///     帐号类型
        /// </summary>
        public AccountType AccountType { get; protected set; }

        /// <summary>
        ///     第三方 UserToken
        /// </summary>
        public string UserToken { get; set; }

        /// <summary>
        ///     当前登陆了帐号标识
        /// </summary>
        public string AccountId { get; protected set; }

        /// <summary>
        ///     用户是否可用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     User.Id
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        ///     User.Name
        /// </summary>
        public string UserName { get; set; }
    }
}