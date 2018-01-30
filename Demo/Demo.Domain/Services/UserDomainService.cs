﻿using System;
using System.Threading.Tasks;
using Demo.Domain.Models.Users;
using Demo.Domain.Repositories;
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

        public async Task<User> RegisterUserAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            }

            var userExists = await _repository.ExistsAsync(new UserNameSpec(userName))
                                              .ConfigureAwait(false);
            if (userExists)
            {
                throw new DomainException(Error.UserNameAlreadyExists, new object[] {userName});
            }
            var user = new User(userName);
            password = _encryptService.EncryptPassword(password);
            var account = user.CreateAccount(password);
            _repository.Add(user);
            _repository.Add(account);
            return user;
        }
    }
}