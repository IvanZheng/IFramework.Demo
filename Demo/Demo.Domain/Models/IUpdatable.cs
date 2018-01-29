using Demo.Domain.Models.Users;

namespace Demo.Domain.Models
{
    public interface IUpdatable
    {
        void UpdateModification(UserInfo modifier);
        void UpdateCreation(UserInfo modifier);
    }
}