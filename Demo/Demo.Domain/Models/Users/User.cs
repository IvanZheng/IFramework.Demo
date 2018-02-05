using System;
using Demo.Domain.Models.Accounts;
using Demo.DTO.Enums;
using IFramework.Exceptions;
using IFramework.Infrastructure;

namespace Demo.Domain.Models.Users
{
    /// <summary>
    ///     用户
    /// </summary>
    public class User
    {
        private CommonStatus _status;
        protected User() { }

        public User(string name,
                    string phone = null,
                    string country = null,
                    string province = null,
                    string city = null,
                    string headerImage = null)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Country = country;
            Phone = phone;
            Province = province;
            City = city;
            HeaderImage = headerImage;
        }

        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public string Phone { get; protected set; }
        public string Gender { get; protected set; }

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

        public string Country { get; protected set; }
        public string Province { get; protected set; }
        public string City { get; protected set; }
        public string HeaderImage { get; protected set; }
        public string RealName { get; protected set; }
        public string Contact { get; protected set; }
        public bool Enabled => Status == CommonStatus.Normal;

        public Account CreateAccount(long accountId, string password)
        {
            return new Account(accountId, Id, Name, password);
        }

        public UserInfo GetInfo()
        {
            return new UserInfo(Id, Name);
        }

        public void Update(string name, string country, string province, string city, string gender, string headImage)
        {
            Gender = gender;
            Country = country;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Province = province;
            City = city;
            HeaderImage = headImage;
            Status = CommonStatus.Normal;
        }

        public void Complete(string realName, string contact)
        {
            RealName = realName;
            Contact = contact;
        }
    }
}