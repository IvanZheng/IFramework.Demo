using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IFramework.Specifications;

namespace Demo.Domain.Models.Users
{
    public class UserNameSpec: Specification<User>
    {
        public string Name { get; protected set; }

        public UserNameSpec(string name)
        {
            Name = name;
        }
        public override Expression<Func<User, bool>> GetExpression()
        {
            return u => u.Name == Name;
        }
    }
}
