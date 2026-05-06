using System;
using System.Collections.Generic;
using System.Linq;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;

namespace testSrorage.Application
{
    public sealed class StorageService : IStorageService
    {
        private readonly IRepository<Product> products;
        private readonly IRepository<Arrival> arrivals;
        private readonly IRepository<Expense> expenses;

        public StorageService(
            IRepository<Product> products,
            IRepository<Arrival> arrivals,
            IRepository<Expense> expenses)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));
            if (arrivals == null)
                throw new ArgumentNullException(nameof(arrivals));
            if (expenses == null)
                throw new ArgumentNullException(nameof(expenses));

            this.products = products;
            this.arrivals = arrivals;
            this.expenses = expenses;
        }

        public OperationResult CheckConnection()
        {
            try
            {
                products.GetAll();
                return OperationResult.Ok("Соединение с базой данных успешно.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка подключения к базе данных: " + ex.Message);
            }
        }

        public List<Product> GetProducts()
        {
            return products.GetAll();
        }

        public List<Arrival> GetArrivals()
        {
            return arrivals.GetAll();
        }

        public List<Expense> GetExpenses()
        {
            return expenses.GetAll();
        }

        public OperationResult AddArrival(string productName, int quantity, DateTime arrivalDate)
        {
            try
            {
                OperationResult validation = ValidateProductOperation(productName, quantity);
                if (!validation.Success)
                    return validation;

                Product product = FindProductByName(productName);
                string message;

                if (product == null)
                {
                    product = new Product
                    {
                        Name = productName.Trim(),
                        Quantity = quantity
                    };

                    products.Insert(product);
                    message = "Создан новый продукт.";
                }
                else
                {
                    product.Quantity += quantity;
                    products.Update(product);
                    message = "Количество существующего продукта обновлено.";
                }

                arrivals.Insert(new Arrival
                {
                    DateTime = arrivalDate.Date,
                    ProductId = product.Id,
                    Quantity = quantity
                });

                return OperationResult.Ok(message + " Приход успешно добавлен.");
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

                Product product = FindProductByName(productName);
                if (product == null)
                    return OperationResult.Fail("Товар '" + productName.Trim() + "' не найден в базе данных.");

                if (product.Quantity < quantity)
                    return OperationResult.Fail("Невозможно провести расход: требуется " + quantity + ", на складе " + product.Quantity + ".");

                product.Quantity -= quantity;
                products.Update(product);

                expenses.Insert(new Expense
                {
                    DateTime = expenseDate.Date,
                    ProductId = product.Id,
                    Quantity = quantity
                });

                return OperationResult.Ok("Расход успешно добавлен.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка добавления расхода: " + ex.Message);
            }
        }

        public OperationResult UpdateProduct(Product product)
        {
            try
            {
                if (product == null)
                    return OperationResult.Fail("Продукт не выбран.");

                if (string.IsNullOrWhiteSpace(product.Name))
                    return OperationResult.Fail("Введите название продукта.");

                if (product.Quantity < 0)
                    return OperationResult.Fail("Количество продукта не может быть меньше 0.");

                return products.Update(product)
                    ? OperationResult.Ok("Продукт успешно обновлен.")
                    : OperationResult.Fail("Не удалось обновить продукт.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка обновления продукта: " + ex.Message);
            }
        }

        public OperationResult UpdateArrival(Arrival arrival)
        {
            try
            {
                OperationResult validation = ValidateDocument(arrival);
                if (!validation.Success)
                    return validation;

                Arrival oldArrival = arrivals.GetById(arrival.Id);
                if (oldArrival == null)
                    return OperationResult.Fail("Приход не найден.");

                if (oldArrival.ProductId == arrival.ProductId)
                {
                    Product product = products.GetById(arrival.ProductId);
                    if (product == null)
                        return OperationResult.Fail("Выбранный продукт не найден.");

                    product.Quantity = product.Quantity - oldArrival.Quantity + arrival.Quantity;
                    if (product.Quantity < 0)
                        product.Quantity = 0;
                    products.Update(product);
                }
                else
                {
                    Product oldProduct = products.GetById(oldArrival.ProductId);
                    Product newProduct = products.GetById(arrival.ProductId);

                    if (newProduct == null)
                        return OperationResult.Fail("Выбранный продукт не найден.");

                    if (oldProduct != null)
                    {
                        oldProduct.Quantity -= oldArrival.Quantity;
                        if (oldProduct.Quantity < 0)
                            oldProduct.Quantity = 0;
                        products.Update(oldProduct);
                    }

                    newProduct.Quantity += arrival.Quantity;
                    products.Update(newProduct);
                }

                arrival.DateTime = arrival.DateTime.Date;
                return arrivals.Update(arrival)
                    ? OperationResult.Ok("Приход обновлен.")
                    : OperationResult.Fail("Не удалось обновить приход.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка обновления прихода: " + ex.Message);
            }
        }

        public OperationResult UpdateExpense(Expense expense)
        {
            try
            {
                OperationResult validation = ValidateDocument(expense);
                if (!validation.Success)
                    return validation;

                Expense oldExpense = expenses.GetById(expense.Id);
                if (oldExpense == null)
                    return OperationResult.Fail("Расход не найден.");

                if (oldExpense.ProductId == expense.ProductId)
                {
                    Product product = products.GetById(expense.ProductId);
                    if (product == null)
                        return OperationResult.Fail("Выбранный продукт не найден.");

                    int availableQuantity = product.Quantity + oldExpense.Quantity;
                    if (availableQuantity < expense.Quantity)
                        return OperationResult.Fail("Недостаточно товара на складе для обновления расхода.");

                    product.Quantity = availableQuantity - expense.Quantity;
                    products.Update(product);
                }
                else
                {
                    Product oldProduct = products.GetById(oldExpense.ProductId);
                    Product newProduct = products.GetById(expense.ProductId);

                    if (newProduct == null)
                        return OperationResult.Fail("Выбранный продукт не найден.");

                    if (newProduct.Quantity < expense.Quantity)
                        return OperationResult.Fail("Недостаточно товара на складе для обновления расхода.");

                    if (oldProduct != null)
                    {
                        oldProduct.Quantity += oldExpense.Quantity;
                        products.Update(oldProduct);
                    }

                    newProduct.Quantity -= expense.Quantity;
                    products.Update(newProduct);
                }

                expense.DateTime = expense.DateTime.Date;
                return expenses.Update(expense)
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
            int arrivalCount = arrivals.GetAll().Count(item => item.ProductId == productId);
            int expenseCount = expenses.GetAll().Count(item => item.ProductId == productId);
            return arrivalCount + expenseCount;
        }

        public OperationResult DeleteProduct(Product product)
        {
            try
            {
                if (product == null)
                    return OperationResult.Fail("Продукт не выбран.");

                List<Arrival> productArrivals = arrivals.GetAll()
                    .Where(item => item.ProductId == product.Id)
                    .ToList();
                List<Expense> productExpenses = expenses.GetAll()
                    .Where(item => item.ProductId == product.Id)
                    .ToList();

                foreach (Arrival arrival in productArrivals)
                    arrivals.Delete(arrival.Id);

                foreach (Expense expense in productExpenses)
                    expenses.Delete(expense.Id);

                return products.Delete(product.Id)
                    ? OperationResult.Ok("Продукт и связанные записи удалены.")
                    : OperationResult.Fail("Не удалось удалить продукт.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка удаления продукта: " + ex.Message);
            }
        }

        public OperationResult DeleteArrival(Arrival arrival)
        {
            try
            {
                if (arrival == null)
                    return OperationResult.Fail("Приход не выбран.");

                Product product = products.GetById(arrival.ProductId);
                if (product != null)
                {
                    product.Quantity -= arrival.Quantity;
                    if (product.Quantity < 0)
                        product.Quantity = 0;
                    products.Update(product);
                }

                return arrivals.Delete(arrival.Id)
                    ? OperationResult.Ok("Приход удален.")
                    : OperationResult.Fail("Не удалось удалить приход.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка удаления прихода: " + ex.Message);
            }
        }

        public OperationResult DeleteExpense(Expense expense)
        {
            try
            {
                if (expense == null)
                    return OperationResult.Fail("Расход не выбран.");

                Product product = products.GetById(expense.ProductId);
                if (product != null)
                {
                    product.Quantity += expense.Quantity;
                    products.Update(product);
                }

                return expenses.Delete(expense.Id)
                    ? OperationResult.Ok("Расход удален.")
                    : OperationResult.Fail("Не удалось удалить расход.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка удаления расхода: " + ex.Message);
            }
        }

        public List<Arrival> GetArrivalsByDateRange(DateTime startDate, DateTime endDate)
        {
            DateTime start = startDate.Date;
            DateTime end = endDate.Date;

            return arrivals.GetAll()
                .Where(item => item.DateTime.Date >= start && item.DateTime.Date <= end)
                .OrderByDescending(item => item.DateTime)
                .ToList();
        }

        public List<Expense> GetExpensesByDateRange(DateTime startDate, DateTime endDate)
        {
            DateTime start = startDate.Date;
            DateTime end = endDate.Date;

            return expenses.GetAll()
                .Where(item => item.DateTime.Date >= start && item.DateTime.Date <= end)
                .OrderByDescending(item => item.DateTime)
                .ToList();
        }

        public ReportSummary CreateSummary<TDocument>(IEnumerable<TDocument> documents)
            where TDocument : IStorageDocument
        {
            if (documents == null)
                return new ReportSummary(0, 0);

            List<TDocument> items = documents.ToList();
            return new ReportSummary(items.Sum(document => document.Quantity), items.Count);
        }

        private Product FindProductByName(string productName)
        {
            string normalizedName = productName.Trim();

            return products.GetAll()
                .FirstOrDefault(item => string.Equals(
                    item.Name,
                    normalizedName,
                    StringComparison.OrdinalIgnoreCase));
        }

        private static OperationResult ValidateProductOperation(string productName, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return OperationResult.Fail("Введите имя товара.");

            if (quantity <= 0)
                return OperationResult.Fail("Количество должно быть больше 0.");

            return OperationResult.Ok(string.Empty);
        }

        private static OperationResult ValidateDocument(IStorageDocument document)
        {
            if (document == null)
                return OperationResult.Fail("Запись не выбрана.");

            if (document.ProductId <= 0)
                return OperationResult.Fail("Выберите продукт.");

            if (document.Quantity <= 0)
                return OperationResult.Fail("Количество должно быть больше 0.");

            return OperationResult.Ok(string.Empty);
        }
    }
}
