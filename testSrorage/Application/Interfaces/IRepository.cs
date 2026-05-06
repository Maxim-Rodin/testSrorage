using System.Collections.Generic;
using testSrorage.Domain;

namespace testSrorage.Application.Interfaces
{
    // Общий интерфейс репозитория для CRUD операций над сущностью T.
        public interface IRepository<T>
            where T : BaseEntity, new()
        {
            // Возвращает все записи.
            List<T> GetAll();
            // Возвращает запись по Id.
            T GetById(int id);
            // Вставляет запись и возвращает новый Id.
            int Insert(T entity);
            // Обновляет запись.
            bool Update(T entity);
            // Удаляет запись по Id.
            bool Delete(int id);
        }
}
