using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;

namespace testSrorage.Application
{
    public sealed class StorageService : IStorageService
    {
        private readonly IRepositoryFactory repositoryFactory;

        public StorageService(IRepositoryFactory repositoryFactory)
        {
            if (repositoryFactory == null)
                throw new ArgumentNullException(nameof(repositoryFactory));

            this.repositoryFactory = repositoryFactory;
        }

        public OperationResult CheckConnection()
        {
            try
            {
                repositoryFactory.GetRepository<Product>().GetAll();
                return OperationResult.Ok("Соединение с базой данных успешно.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка подключения к базе данных: " + ex.Message);
            }
        }

        // Generic CRUD
        public List<T> GetAll<T>() where T : BaseEntity, new()
        {
            return repositoryFactory.GetRepository<T>().GetAll();
        }

        public T GetById<T>(int id) where T : BaseEntity, new()
        {
            return repositoryFactory.GetRepository<T>().GetById(id);
        }

        public OperationResult Insert<T>(T entity) where T : BaseEntity, new()
        {
            try
            {
                int id = repositoryFactory.GetRepository<T>().Insert(entity);
                entity.Id = id;
                return OperationResult.Ok("Элемент добавлен.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка добавления: " + ex.Message);
            }
        }

        public OperationResult Update<T>(T entity) where T : BaseEntity, new()
        {
            try
            {
                bool ok = repositoryFactory.GetRepository<T>().Update(entity);
                return ok ? OperationResult.Ok("Элемент обновлён.") : OperationResult.Fail("Не удалось обновить элемент.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка обновления: " + ex.Message);
            }
        }

        // Изменённый универсальный Delete: если T == Product — выполняем каскадное удаление связанных документов
        public OperationResult Delete<T>(int id) where T : BaseEntity, new()
        {
            try
            {
                if (typeof(T) == typeof(Product))
                {
                    IRepository<Product> productRepo = repositoryFactory.GetRepository<Product>();
                    Product product = productRepo.GetById(id);
                    if (product == null)
                        return OperationResult.Fail("Продукт не найден.");

                    return DeleteProduct(product);
                }

                bool ok = repositoryFactory.GetRepository<T>().Delete(id);
                return ok ? OperationResult.Ok("Элемент удалён.") : OperationResult.Fail("Не удалось удалить элемент.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка удаления: " + ex.Message);
            }
        }

        // Document operations (оставлены без изменений)
        public OperationResult AddDocument<TDocument>(string productName, int quantity, DateTime date)
            where TDocument : BaseEntity, IStorageDocument, new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productName))
                    return OperationResult.Fail("Введите имя товара.");
                if (quantity <= 0)
                    return OperationResult.Fail("Количество должно быть больше 0.");

                IRepository<Product> productRepo = repositoryFactory.GetRepository<Product>();
                Product product = FindProductByName(productRepo, productName);

                bool isIncoming = typeof(IIncomingDocument).IsAssignableFrom(typeof(TDocument));
                bool isOutgoing = typeof(IOutgoingDocument).IsAssignableFrom(typeof(TDocument));

                if (isIncoming)
                {
                    if (product == null)
                    {
                        product = new Product
                        {
                            Name = productName.Trim(),
                            Quantity = quantity
                        };
                        productRepo.Insert(product);
                    }
                    else
                    {
                        product.Quantity += quantity;
                        productRepo.Update(product);
                    }
                }
                else if (isOutgoing)
                {
                    if (product == null)
                        return OperationResult.Fail("Товар '" + productName.Trim() + "' не найден в базе данных.");

                    if (product.Quantity < quantity)
                        return OperationResult.Fail("Невозможно провести операцию: требуется " + quantity + ", на складе " + product.Quantity + ".");

                    product.Quantity -= quantity;
                    productRepo.Update(product);
                }
                else
                {
                    return OperationResult.Fail("Тип документа не поддерживает автоматическое изменение остатков. Добавьте маркерный интерфейс IIncomingDocument или IOutgoingDocument.");
                }

                TDocument doc = new TDocument
                {
                    DateTime = date.Date,
                    ProductId = product.Id,
                    Quantity = quantity
                };

                repositoryFactory.GetRepository<TDocument>().Insert(doc);

                string action = isIncoming ? "Добавлен приход." : "Добавлен расход.";
                return OperationResult.Ok(action);
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка добавления документа: " + ex.Message);
            }
        }

        public OperationResult UpdateDocument<TDocument>(TDocument document)
            where TDocument : BaseEntity, IStorageDocument, new()
        {
            try
            {
                if (document == null)
                    return OperationResult.Fail("Запись не выбрана.");
                if (document.ProductId <= 0)
                    return OperationResult.Fail("Выберите продукт.");
                if (document.Quantity <= 0)
                    return OperationResult.Fail("Количество должно быть больше 0.");

                IRepository<TDocument> docRepo = repositoryFactory.GetRepository<TDocument>();
                TDocument old = docRepo.GetById(document.Id);
                if (old == null)
                    return OperationResult.Fail("Запись не найдена.");

                IRepository<Product> productRepo = repositoryFactory.GetRepository<Product>();
                Product oldProduct = productRepo.GetById(old.ProductId);
                Product newProduct = productRepo.GetById(document.ProductId);

                bool isIncoming = typeof(IIncomingDocument).IsAssignableFrom(typeof(TDocument));
                bool isOutgoing = typeof(IOutgoingDocument).IsAssignableFrom(typeof(TDocument));

                if (!isIncoming && !isOutgoing)
                    return OperationResult.Fail("Тип документа не поддерживает автоматическое изменение остатков.");

                if (old.ProductId == document.ProductId)
                {
                    if (newProduct == null)
                        return OperationResult.Fail("Продукт не найден.");

                    if (isIncoming)
                    {
                        int newQty = newProduct.Quantity - old.Quantity + document.Quantity;
                        newProduct.Quantity = newQty < 0 ? 0 : newQty;
                        productRepo.Update(newProduct);
                    }
                    else
                    {
                        int available = newProduct.Quantity + old.Quantity;
                        if (available < document.Quantity)
                            return OperationResult.Fail("Недостаточно товара на складе для обновления документа.");
                        newProduct.Quantity = available - document.Quantity;
                        productRepo.Update(newProduct);
                    }
                }
                else
                {
                    if (oldProduct != null)
                    {
                        if (isIncoming)
                        {
                            oldProduct.Quantity -= old.Quantity;
                            if (oldProduct.Quantity < 0) oldProduct.Quantity = 0;
                            productRepo.Update(oldProduct);
                        }
                        else
                        {
                            oldProduct.Quantity += old.Quantity;
                            productRepo.Update(oldProduct);
                        }
                    }

                    if (newProduct == null)
                        return OperationResult.Fail("Новый продукт не найден.");

                    if (isIncoming)
                    {
                        newProduct.Quantity += document.Quantity;
                    }
                    else
                    {
                        if (newProduct.Quantity < document.Quantity)
                            return OperationResult.Fail("Недостаточно товара на складе для обновления документа.");
                        newProduct.Quantity -= document.Quantity;
                    }

                    productRepo.Update(newProduct);
                }

                document.DateTime = document.DateTime.Date;
                return docRepo.Update(document)
                    ? OperationResult.Ok("Документ обновлён.")
                    : OperationResult.Fail("Не удалось обновить документ.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка обновления документа: " + ex.Message);
            }
        }

        public List<TDocument> GetDocumentsByDateRange<TDocument>(DateTime startDate, DateTime endDate)
            where TDocument : BaseEntity, IStorageDocument, new()
        {
            DateTime start = startDate.Date;
            DateTime end = endDate.Date;
            return repositoryFactory.GetRepository<TDocument>()
                .GetAll()
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
            return new ReportSummary(items.Sum(d => d.Quantity), items.Count);
        }

        // CountProductDocuments и DeleteProduct оставлены как вспомогательные (Delete<T> вызывает DeleteProduct для Product)
        // Исправление для CS0311: используйте тип t вместо object при вызове методов через reflection
        public int CountProductDocuments(int productId)
        {
            try
            {
                int sum = 0;
                Assembly asm = typeof(Product).Assembly;
                var docTypes = asm.GetTypes()
                    .Where(t => typeof(IStorageDocument).IsAssignableFrom(t) && typeof(BaseEntity).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToList();

                foreach (Type t in docTypes)
                {
                    object repo = repositoryFactory.GetType()
                        .GetMethod(nameof(IRepositoryFactory.GetRepository))
                        .MakeGenericMethod(t)
                        .Invoke(repositoryFactory, null);

                    MethodInfo getAll = repo.GetType().GetMethod("GetAll");
                    var list = (IEnumerable)getAll.Invoke(repo, null);
                    foreach (var item in list)
                    {
                        PropertyInfo pid = item.GetType().GetProperty(nameof(IStorageDocument.ProductId));
                        if (pid != null)
                        {
                            object value = pid.GetValue(item, null);
                            if (value is int v && v == productId)
                                sum++;
                        }
                    }
                }

                return sum;
            }
            catch
            {
                return 0;
            }
        }

        public OperationResult DeleteProduct(Product product)
        {
            try
            {
                if (product == null)
                    return OperationResult.Fail("Продукт не выбран.");

                Assembly asm = typeof(Product).Assembly;
                var docTypes = asm.GetTypes()
                    .Where(t => typeof(IStorageDocument).IsAssignableFrom(t) && typeof(BaseEntity).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToList();

                foreach (Type t in docTypes)
                {
                    object repo = repositoryFactory.GetType()
                        .GetMethod(nameof(IRepositoryFactory.GetRepository))
                        .MakeGenericMethod(t)
                        .Invoke(repositoryFactory, null);

                    MethodInfo getAll = repo.GetType().GetMethod("GetAll");
                    var list = (IEnumerable)getAll.Invoke(repo, null);

                    List<int> ids = new List<int>();
                    foreach (var item in list)
                    {
                        PropertyInfo pid = item.GetType().GetProperty(nameof(IStorageDocument.ProductId));
                        PropertyInfo idProp = item.GetType().GetProperty(nameof(BaseEntity.Id));
                        if (pid != null && idProp != null)
                        {
                            object value = pid.GetValue(item, null);
                            if (value is int v && v == product.Id)
                                ids.Add((int)idProp.GetValue(item, null));
                        }
                    }

                    MethodInfo deleteMethod = repo.GetType().GetMethod("Delete");
                    foreach (int id in ids)
                        deleteMethod.Invoke(repo, new object[] { id });
                }

                bool ok = repositoryFactory.GetRepository<Product>().Delete(product.Id);
                return ok ? OperationResult.Ok("Продукт и связанные записи удалены.") : OperationResult.Fail("Не удалось удалить продукт.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка удаления продукта: " + ex.Message);
            }
        }

        // Helper
        private Product FindProductByName(IRepository<Product> repo, string productName)
        {
            string normalized = productName.Trim();
            return repo.GetAll()
                .FirstOrDefault(p => string.Equals(p.Name, normalized, StringComparison.OrdinalIgnoreCase));
        }
    }
}
