using System;
using System.Threading.Tasks;
using Demo.Domain.Models.Accounts;
using Demo.Domain.Models.Users;
using Demo.Domain.Repositories;
using Demo.DTO.Enums;
using IFramework.Exceptions;
using IFramework.Infrastructure;

namespace Demo.Domain.Services
{
    public class UserDomainService
    {
        private readonly IDemoRepository _repository;
        private readonly IEncryptService _encryptService;

        public UserDomainService(IDemoRepository repository,
                                 IEncryptService encryptService)
        {
            _repository = repository;
            _encryptService = encryptService;
        }

        public async Task<User> RegisterUserAsync(long accountId, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            }

            var userExists = await _repository.ExistsAsync(new AccountExistsSpec(AccountType.Internal, userName))
                                              .ConfigureAwait(false);
            if (userExists)
            {
                throw new DomainException(Error.UserNameAlreadyExists, new object[] {userName});
            }
            return RegisterUser(accountId, userName, password);
        }

        public User RegisterUser(long id, string userName, string password)
        {
            var user = new User(userName);
            //password = _encryptService.EncryptPassword(password);
            var account = user.CreateAccount(id, password);
            //_repository.Add(user);
            _repository.Add(account);
            return user;
        }
    }
}