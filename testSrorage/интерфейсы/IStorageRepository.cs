using System;
using System.Collections.Generic;
using testSrorage.классы;

namespace testSrorage.классы.интерфейсы
{
    internal interface IStorageRepository
    {
        int LastInsertedId { get; }

        List<Products> GetAllProducts();
        Products GetProductById(int id);
        Products GetProductByName(string name);
        bool AddNewProductWithQuantity(string productName, int quantity);
        bool UpdateProduct(Products product);
        void UpdateProductQuantity(int id, int quantity, bool isAddition);
        int GetMaxProductId();

        List<Arrivals> GetAllArrivals();
        bool AddArrivals(Arrivals arrivals);
        bool UpdateArrival(Arrivals arrival);

        List<Expenses> GetAllExpenses();
        bool AddExpenses(Expenses expenses);
        bool UpdateExpense(Expenses expense);

        int CountProductDocuments(int productId);
        bool DeleteProduct(Products product);
        bool DeleteArrival(int arrivalId);
        bool DeleteExpense(int expenseId);

        List<Arrivals> GetArrivalsByDateRange(DateTime startDate, DateTime endDate);
        List<Expenses> GetExpensesByDateRange(DateTime startDate, DateTime endDate);
    }
}
