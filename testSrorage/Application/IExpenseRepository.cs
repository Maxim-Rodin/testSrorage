using System.Collections.Generic;
using testSrorage.классы;

namespace testSrorage.Application
{
    public interface IExpenseRepository
    {
        List<Expense> GetAll();

        void Add(Expense expense);

        void Delete(int id);

        void DeleteByProductId(int productId);
    }
}