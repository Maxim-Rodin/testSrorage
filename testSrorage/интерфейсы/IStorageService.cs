using System;
using System.Collections.Generic;
using testSrorage.классы;

namespace testSrorage.классы.интерфейсы
{
    internal interface IStorageService
    {
        OperationResult CheckConnection();

        List<Products> GetProducts();
        List<Arrivals> GetArrivals();
        List<Expenses> GetExpenses();

        OperationResult AddArrival(string productName, int quantity, DateTime arrivalDate);
        OperationResult AddExpense(string productName, int quantity, DateTime expenseDate);
        OperationResult UpdateProduct(Products product);
        OperationResult UpdateArrival(Arrivals arrival);
        OperationResult UpdateExpense(Expenses expense);

        int CountProductDocuments(int productId);
        OperationResult DeleteProduct(Products product);
        OperationResult DeleteArrival(Arrivals arrival);
        OperationResult DeleteExpense(Expenses expense);

        List<Arrivals> GetArrivalsByDateRange(DateTime startDate, DateTime endDate);
        List<Expenses> GetExpensesByDateRange(DateTime startDate, DateTime endDate);
        ReportSummary CreateSummary<TDocument>(IEnumerable<TDocument> documents)
            where TDocument : IDocument;
    }
}
