using System.Collections.Generic;
using testSrorage.Application;

using testSrorage.классы;

namespace testSrorage.Service
{
    internal class ProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IArrivalRepository _arrivalRepo;
        private readonly IExpenseRepository _expenseRepo;

        public ProductService(
            IProductRepository productRepo,
            IArrivalRepository arrivalRepo,
            IExpenseRepository expenseRepo)
        {
            _productRepo = productRepo;
            _arrivalRepo = arrivalRepo;
            _expenseRepo = expenseRepo;
        }

        // 📦 получить все продукты
        public List<Product> GetAll()
        {
            return _productRepo.GetAll();
        }

        // ➕ создать продукт
        public void CreateProduct(string name, int quantity)
        {
            Product product = new Product(name, quantity);
            _productRepo.Add(product);
        }

        // ✏ обновить продукт
        public void UpdateProduct(Product product)
        {
            _productRepo.Update(product);
        }

        // ❌ удалить продукт + каскад
        public void DeleteProduct(int productId)
        {
            _arrivalRepo.DeleteByProductId(productId);
            _expenseRepo.DeleteByProductId(productId);

            _productRepo.Delete(productId);
        }

        // 📥 приход товара
        public void AddStock(int productId, int amount)
        {
            Product product = _productRepo.GetById(productId);

            product.Increase(amount);
            _productRepo.Update(product);

            Arrival arrival = new Arrival(productId, amount);
            _arrivalRepo.Add(arrival);
        }

        // 📤 расход товара
        public void RemoveStock(int productId, int amount)
        {
            Product product = _productRepo.GetById(productId);

            product.Decrease(amount);
            _productRepo.Update(product);

            Expense expense = new Expense(productId, amount);
            _expenseRepo.Add(expense);
        }
    }
}