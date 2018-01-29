using System;
using Demo.Domain.Models.Users;

namespace Demo.Domain.Models
{
    public abstract class AggregateRoot : IFramework.Domain.AggregateRoot, IUpdatable
    {
        public DateTime CreatedTime { get; protected set; }
        public DateTime ModifiedTime { get; protected set; }
        public UserInfo Creator { get; protected set; }
        public UserInfo Modifier { get; protected set; }


        public void UpdateCreation(UserInfo creator)
        {
            CreatedTime = ModifiedTime = DateTime.Now;
            Creator = creator.Clone();
            Modifier = creator.Clone();
        }

        public void UpdateModification(UserInfo modifier)
        {
            Modifier = modifier.Clone();
            ModifiedTime = DateTime.Now;
        }
    }

    public class TimestampedAggregateRoot : IFramework.Domain.TimestampedAggregateRoot, IUpdatable
    {
        public DateTime CreatedTime { get; protected set; }
        public DateTime ModifiedTime { get; protected set; }
        public UserInfo Creator { get; protected set; }
        public UserInfo Modifier { get; protected set; }

        public void UpdateCreation(UserInfo creator)
        {
            CreatedTime = ModifiedTime = DateTime.Now;
            Creator = creator.Clone();
            Modifier = creator.Clone();
        }

        public void UpdateModification(UserInfo modifier)
        {
            Modifier = modifier.Clone();
            ModifiedTime = DateTime.Now;
        }
    }
}