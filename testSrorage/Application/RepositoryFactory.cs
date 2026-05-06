using System;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;

namespace testSrorage.Infrastructure
{
    // Фабрика репозиториев: создаёт репозиторий для заданного типа сущности.
    public sealed class RepositoryFactory : IRepositoryFactory
    {
        private readonly DbConnectionFactory connectionFactory;

        // Конструктор — принимает фабрику подключений.
        public RepositoryFactory(DbConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            this.connectionFactory = connectionFactory;
        }

        // Возвращает IRepository<T> для типа T.
        public IRepository<T> GetRepository<T>()
            where T : BaseEntity, new()
        {
            return new GenericRepository<T>(connectionFactory);
        }
    }
}
