using IFramework.Domain;

namespace Demo.Domain.Models.Users
{
    public class UserInfo : ValueObject<UserInfo>
    {
        public static UserInfo Null => new UserInfo();

        public UserInfo() { }

        public UserInfo(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; protected set; }
        public string Name { get; protected set; }
    }
}