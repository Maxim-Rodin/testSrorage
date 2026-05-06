using System;
using System.Collections.Generic;
using System.Linq;
using testSrorage.классы.интерфейсы;

namespace testSrorage.классы
{
    internal sealed class StorageService : IStorageService
    {
        private readonly IStorageRepository repository;

        public StorageService()
            : this(new DBManager())
        {
        }

        public StorageService(IStorageRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            this.repository = repository;
        }

        public OperationResult CheckConnection()
        {
            try
            {
                repository.GetAllProducts();
                return OperationResult.Ok("Соединение с базой данных успешно.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка подключения к базе данных: " + ex.Message);
            }
        }

        public List<Products> GetProducts()
        {
            return repository.GetAllProducts();
        }

        public List<Arrivals> GetArrivals()
        {
            return repository.GetAllArrivals();
        }

        public List<Expenses> GetExpenses()
        {
            return repository.GetAllExpenses();
        }

        public OperationResult AddArrival(string productName, int quantity, DateTime arrivalDate)
        {
            try
            {
                OperationResult validation = ValidateProductOperation(productName, quantity);
                if (!validation.Success)
                    return validation;

                string normalizedName = productName.Trim();
                Products product = repository.GetProductByName(normalizedName);
                int productId;
                string productMessage;

                if (product == null)
                {
                    if (!repository.AddNewProductWithQuantity(normalizedName, quantity))
                        return OperationResult.Fail("Не удалось создать новый продукт.");

                    productId = repository.LastInsertedId > 0
                        ? repository.LastInsertedId
                        : repository.GetMaxProductId();
                    productMessage = "Создан новый продукт.";
                }
                else
                {
                    productId = product.IdProduct;
                    repository.UpdateProductQuantity(productId, quantity, true);
                    productMessage = "Количество существующего продукта обновлено.";
                }

                Arrivals arrival = new Arrivals
                {
                    DateTime = arrivalDate.Date,
                    ProductId = productId,
                    Quantity = quantity
                };

                return repository.AddArrivals(arrival)
                    ? OperationResult.Ok(productMessage + " Приход успешно добавлен.")
                    : OperationResult.Fail("Не удалось сохранить приход.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка добавления прихода: " + ex.Message);
            }
        }

        public OperationResult AddExpense(string productName, int quantity, DateTime expenseDate)
        {
            try
            {
                OperationResult validation = ValidateProductOperation(productName, quantity);
                if (!validation.Success)
                    return validation;

                string normalizedName = productName.Trim();
                Products product = repository.GetProductByName(normalizedName);

                if (product == null)
                    return OperationResult.Fail("Товар '" + normalizedName + "' не найден в базе данных.");

                if (product.Quantity < quantity)
                {
                    return OperationResult.Fail(
                        "Невозможно провести расход: требуется " + quantity + ", на складе " + product.Quantity + ".");
                }

                repository.UpdateProductQuantity(product.IdProduct, quantity, false);

                Expenses expense = new Expenses
                {
                    DateTime = expenseDate.Date,
                    ProductId = product.IdProduct,
                    Quantity = quantity
                };

                return repository.AddExpenses(expense)
                    ? OperationResult.Ok("Расход успешно добавлен.")
                    : OperationResult.Fail("Не удалось сохранить расход.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка добавления расхода: " + ex.Message);
            }
        }

        public OperationResult UpdateProduct(Products product)
        {
            try
            {
                if (product == null)
                    return OperationResult.Fail("Продукт не выбран.");

                if (string.IsNullOrWhiteSpace(product.Name))
                    return OperationResult.Fail("Введите название продукта.");

                if (product.Quantity < 0)
                    return OperationResult.Fail("Количество продукта не может быть меньше 0.");

                return repository.UpdateProduct(product)
                    ? OperationResult.Ok("Продукт успешно обновлен.")
                    : OperationResult.Fail("Не удалось обновить продукт.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка обновления продукта: " + ex.Message);
            }
        }

        public OperationResult UpdateArrival(Arrivals arrival)
        {
            try
            {
                OperationResult validation = ValidateDocument(arrival, "приход");
                if (!validation.Success)
                    return validation;

                return repository.UpdateArrival(arrival)
                    ? OperationResult.Ok("Приход обновлен.")
                    : OperationResult.Fail("Не удалось обновить приход.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка обновления прихода: " + ex.Message);
            }
        }

        public OperationResult UpdateExpense(Expenses expense)
        {
            try
            {
                OperationResult validation = ValidateDocument(expense, "расход");
                if (!validation.Success)
                    return validation;

                return repository.UpdateExpense(expense)
                    ? OperationResult.Ok("Расход обновлен.")
                    : OperationResult.Fail("Не удалось обновить расход.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка обновления расхода: " + ex.Message);
            }
        }

        public int CountProductDocuments(int productId)
        {
            return repository.CountProductDocuments(productId);
        }

        public OperationResult DeleteProduct(Products product)
        {
            try
            {
                if (product == null)
                    return OperationResult.Fail("Продукт не выбран.");

                return repository.DeleteProduct(product)
                    ? OperationResult.Ok("Продукт удален.")
                    : OperationResult.Fail("Не удалось удалить продукт.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка удаления продукта: " + ex.Message);
            }
        }

        public OperationResult DeleteArrival(Arrivals arrival)
        {
            try
            {
                if (arrival == null)
                    return OperationResult.Fail("Приход не выбран.");

                return repository.DeleteArrival(arrival.IdArrivals)
                    ? OperationResult.Ok("Приход удален.")
                    : OperationResult.Fail("Не удалось удалить приход.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка удаления прихода: " + ex.Message);
            }
        }

        public OperationResult DeleteExpense(Expenses expense)
        {
            try
            {
                if (expense == null)
                    return OperationResult.Fail("Расход не выбран.");

                return repository.DeleteExpense(expense.IdExpenses)
                    ? OperationResult.Ok("Расход удален.")
                    : OperationResult.Fail("Не удалось удалить расход.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка удаления расхода: " + ex.Message);
            }
        }

        public List<Arrivals> GetArrivalsByDateRange(DateTime startDate, DateTime endDate)
        {
            return repository.GetArrivalsByDateRange(startDate, endDate);
        }

        public List<Expenses> GetExpensesByDateRange(DateTime startDate, DateTime endDate)
        {
            return repository.GetExpensesByDateRange(startDate, endDate);
        }

        public ReportSummary CreateSummary<TDocument>(IEnumerable<TDocument> documents)
            where TDocument : IDocument
        {
            if (documents == null)
                return new ReportSummary(0, 0);

            List<TDocument> items = documents.ToList();
            return new ReportSummary(items.Sum(document => document.Quantity), items.Count);
        }

        private static OperationResult ValidateProductOperation(string productName, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return OperationResult.Fail("Введите имя товара.");

            if (quantity <= 0)
                return OperationResult.Fail("Количество должно быть больше 0.");

            return OperationResult.Ok(string.Empty);
        }

        private static OperationResult ValidateDocument(IDocument document, string documentName)
        {
            if (document == null)
                return OperationResult.Fail("Запись не выбрана.");

            if (document.ProductId <= 0)
                return OperationResult.Fail("Выберите продукт для записи '" + documentName + "'.");

            if (document.Quantity <= 0)
                return OperationResult.Fail("Количество должно быть больше 0.");

            return OperationResult.Ok(string.Empty);
        }
    }
}
