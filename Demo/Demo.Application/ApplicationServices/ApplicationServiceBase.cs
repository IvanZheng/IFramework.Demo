using System.Threading.Tasks;
using IFramework.UnitOfWork;
using Demo.Domain.Repositories;
using Demo.DTO;
using Demo.Domain.Models.Users;

namespace Demo.Application.ApplicationServices
{
    public class ApplicationServiceBase
    {
        protected IDemoRepository Repository;
        protected IAppUnitOfWork UnitOfWork;

        public ApplicationServiceBase(IDemoRepository repository, IAppUnitOfWork unitOfWork)
        {
            Repository = repository;
            UnitOfWork = unitOfWork;
        }

        protected void Commit(UserInfo user = null)
        {
            UnitOfWork.Commit();
        }

        protected Task CommitAsync(UserInfo user = null)
        {
            return UnitOfWork.CommitAsync();
        }
    }
}