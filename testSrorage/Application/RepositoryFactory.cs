using System;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;

namespace testSrorage.Infrastructure
{
    public sealed class RepositoryFactory : IRepositoryFactory
    {
        private readonly DbConnectionFactory connectionFactory;

        public RepositoryFactory(DbConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            this.connectionFactory = connectionFactory;
        }

        public IRepository<T> GetRepository<T>()
            where T : BaseEntity, new()
        {
            return new GenericRepository<T>(connectionFactory);
        }
    }
}
