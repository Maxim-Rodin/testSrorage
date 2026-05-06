using System;
using System.Collections.Generic;
using testSrorage.Application;
using testSrorage.Domain;

namespace testSrorage.Application.Interfaces
{
    // Интерфейс сервиса работы со складом: CRUD, операции с документами и отчёты.
    public interface IStorageService
    {
        // Проверяет подключение к БД и возвращает результат.
        OperationResult CheckConnection();

        // Generic CRUD
        List<T> GetAll<T>() where T : BaseEntity, new();
        T GetById<T>(int id) where T : BaseEntity, new();
        OperationResult Insert<T>(T entity) where T : BaseEntity, new();
        OperationResult Update<T>(T entity) where T : BaseEntity, new();
        OperationResult Delete<T>(int id) where T : BaseEntity, new();

        // Document-oriented generic operations (for IStorageDocument types)
        // Добавляет документ (приход/расход) для продукта по имени.
        OperationResult AddDocument<TDocument>(string productName, int quantity, DateTime date)
            where TDocument : BaseEntity, IStorageDocument, new();

        // Обновляет документ, корректируя остатки при необходимости.
        OperationResult UpdateDocument<TDocument>(TDocument document)
            where TDocument : BaseEntity, IStorageDocument, new();

        // Возвращает списки документов заданного типа за диапазон дат.
        List<TDocument> GetDocumentsByDateRange<TDocument>(DateTime startDate, DateTime endDate)
            where TDocument : BaseEntity, IStorageDocument, new();

        // Создаёт суммарный отчёт (количество и число записей).
        ReportSummary CreateSummary<TDocument>(IEnumerable<TDocument> documents)
            where TDocument : IStorageDocument;

        // Помощники для UI
        int CountProductDocuments(int productId);
        OperationResult DeleteProduct(Product product);
    }
}
