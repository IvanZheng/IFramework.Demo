using Demo.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFramework.Infrastructure;

namespace Demo.Infrastructure
{
    public class EncryptService : IEncryptService
    {
        public string EncryptPassword(string password, string encryptKey = null)
        {
            return Utility.MD5Encrypt(password);
        }
    }
}
