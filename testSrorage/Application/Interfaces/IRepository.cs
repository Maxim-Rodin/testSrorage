using System.Collections.Generic;
using testSrorage.Domain;

namespace testSrorage.Application.Interfaces
{
    public interface IRepository<T>
        where T : BaseEntity, new()
    {
        List<T> GetAll();
        T GetById(int id);
        int Insert(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
}
