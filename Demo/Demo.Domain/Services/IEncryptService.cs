using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Domain.Services
{
    public interface IEncryptService
    {
        string EncryptPassword(string password, string encryptKey = null);
    }
}
