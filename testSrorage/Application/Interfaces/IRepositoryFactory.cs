using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testSrorage.Domain;

namespace testSrorage.Application.Interfaces
{
    // Фабрика репозиториев: абстракция для получения IRepository<T>.
    public interface IRepositoryFactory
    {
        IRepository<T> GetRepository<T>()
            where T : BaseEntity, new();
    }
}
