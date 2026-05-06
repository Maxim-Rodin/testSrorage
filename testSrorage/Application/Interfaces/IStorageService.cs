using System;
using System.Collections.Generic;
using testSrorage.Application;
using testSrorage.Domain;

namespace testSrorage.Application.Interfaces
{
    public interface IStorageService
    {
        OperationResult CheckConnection();

        // Generic CRUD
        List<T> GetAll<T>() where T : BaseEntity, new();
        T GetById<T>(int id) where T : BaseEntity, new();
        OperationResult Insert<T>(T entity) where T : BaseEntity, new();
        OperationResult Update<T>(T entity) where T : BaseEntity, new();
        OperationResult Delete<T>(int id) where T : BaseEntity, new();

        // Document-oriented generic operations (for IStorageDocument types)
        OperationResult AddDocument<TDocument>(string productName, int quantity, DateTime date)
            where TDocument : BaseEntity, IStorageDocument, new();

        OperationResult UpdateDocument<TDocument>(TDocument document)
            where TDocument : BaseEntity, IStorageDocument, new();

        List<TDocument> GetDocumentsByDateRange<TDocument>(DateTime startDate, DateTime endDate)
            where TDocument : BaseEntity, IStorageDocument, new();

        ReportSummary CreateSummary<TDocument>(IEnumerable<TDocument> documents)
            where TDocument : IStorageDocument;

        // Helpers that remain generic for UI convenience
        int CountProductDocuments(int productId);
        OperationResult DeleteProduct(Product product);
    }
}
