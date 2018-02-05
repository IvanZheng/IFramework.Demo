using System;
using Demo.DTO.Enums;
using IFramework.Exceptions;
using IFramework.Infrastructure;

namespace Demo.Domain.Models.Accounts
{
    /// <summary>
    ///     帐号
    /// </summary>
    public class Account : AggregateRoot
    {
        /// <summary>
        ///     状态
        /// </summary>
        private CommonStatus _status;

        protected Account() { }

        /// <summary>
        /// 用于应用内账号创建
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public Account(long id, string userId, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            }
            Id = id;
            UserName = userName;
            Password = password;
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            AccountType = AccountType.Internal;
            Status = CommonStatus.Normal;
        }

        /// <summary>
        /// 用于第三方,入微信认证登陆
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="outerId"></param>
        public Account(long id, string userId, string outerId)
        {
            Id = id;
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserName = outerId;
            AccountType = AccountType.ThirdParties;
            Status = CommonStatus.Normal;
        }

        /// <summary>
        ///     帐号标识
        /// </summary>
        public long Id { get; protected set; }

        /// <summary>
        ///     用户标识
        /// </summary>
        public string UserId { get; protected set; }

        /// <summary>
        ///     帐号类型: Internal, ThirdParites
        /// </summary>
        public AccountType AccountType { get; protected set; }

        /// <summary>
        ///     用户名  当第三方帐号认证时, UserName为Token
        /// </summary>
        public string UserName { get; protected set; }

        /// <summary>
        ///     密码
        /// </summary>
        public string Password { get; protected set; }

        public CommonStatus Status
        {
            get => _status;
            set
            {
                if (_status == CommonStatus.All)
                {
                    throw new DomainException(Error.InvalidCommonStatus, new object[] {value});
                }
                _status = value;
            }
        }
    }
}