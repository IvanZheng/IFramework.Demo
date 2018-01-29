
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Domain.Models.Users;

namespace Demo.Persistence
{
    public interface ISetUserable
    {
        void SetCurrentUser(UserInfo user);
    }
}
