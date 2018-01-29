using Demo.Domain.Repositories;

namespace Demo.Application.QueryServices
{
    public class QueryServiceBase
    {
        protected IDemoRepository Repository;

        public QueryServiceBase(IDemoRepository repository)
        {
            Repository = repository;
        }
    }
}