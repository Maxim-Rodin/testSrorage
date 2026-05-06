using System.Collections.Generic;
using testSrorage.классы;

namespace testSrorage.Application
{
    public interface IArrivalRepository
    {
        List<Arrival> GetAll();

        void Add(Arrival arrival);

        void Delete(int id);

        void DeleteByProductId(int productId);
        void Update(Arrival arrival);
    }
}