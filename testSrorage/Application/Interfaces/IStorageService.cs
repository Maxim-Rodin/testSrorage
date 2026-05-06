using System;
using System.Collections.Generic;
using testSrorage.Application;
using testSrorage.Domain;

namespace testSrorage.Application.Interfaces
{
    public interface IStorageService
    {
        OperationResult CheckConnection();

        List<Product> GetProducts();
        List<Arrival> GetArrivals();
        List<Expense> GetExpenses();

        OperationResult AddArrival(string productName, int quantity, DateTime arrivalDate);
        OperationResult AddExpense(string productName, int quantity, DateTime expenseDate);
        OperationResult UpdateProduct(Product product);
        OperationResult UpdateArrival(Arrival arrival);
        OperationResult UpdateExpense(Expense expense);

        int CountProductDocuments(int productId);
        OperationResult DeleteProduct(Product product);
        OperationResult DeleteArrival(Arrival arrival);
        OperationResult DeleteExpense(Expense expense);

        List<Arrival> GetArrivalsByDateRange(DateTime startDate, DateTime endDate);
        List<Expense> GetExpensesByDateRange(DateTime startDate, DateTime endDate);
        ReportSummary CreateSummary<TDocument>(IEnumerable<TDocument> documents)
            where TDocument : IStorageDocument;
    }
}
