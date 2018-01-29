
using IFramework.Event;
using IFramework.Infrastructure.Logging;
using IFramework.Message;
using IFramework.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Domain.Models.Users;

namespace Demo.Persistence
{
    public class AppUnitOfWork : IFramework.EntityFramework.AppUnitOfWork, ISetUserable
    {
        public AppUnitOfWork(IEventBus eventBus,
                             ILoggerFactory loggerFactory,
                             IMessagePublisher eventPublisher,
                             IMessageQueueClient messageQueueClient,
                             IMessageStore messageStore)
            : base(eventBus, loggerFactory, eventPublisher, messageQueueClient, messageStore)
        {

        }

        public void SetCurrentUser(UserInfo user)
        {
            _dbContexts.ForEach(dbContext => (dbContext as ISetUserable)?.SetCurrentUser(user));
        }
    }
}
