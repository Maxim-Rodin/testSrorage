using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using testSrorage.классы;
using testSrorage.классы.интерфейсы;

namespace testSrorage
{
    internal class DBManager : IStorageRepository
    {
        private readonly DB connect;

        public DBManager()
            : this(new DB())
        {
        }

        public DBManager(DB connect)
        {
            if (connect == null)
                throw new ArgumentNullException(nameof(connect));

            this.connect = connect;
        }

        public int LastInsertedId { get; private set; }

        public List<Products> GetAllProducts()
        {
            const string query = @"
                SELECT idProducts, nameProduct, quantity
                FROM products
                ORDER BY nameProduct";

            List<Products> products = new List<Products>();
            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        products.Add(ReadProduct(reader));
                }

                return products;
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public Products GetProductById(int id)
        {
            const string query = @"
                SELECT idProducts, nameProduct, quantity
                FROM products
                WHERE idProducts = @id";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.Read() ? ReadProduct(reader) : null;
                    }
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public Products GetProductByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            const string query = @"
                SELECT idProducts, nameProduct, quantity
                FROM products
                WHERE LOWER(nameProduct) = LOWER(@name)
                LIMIT 1";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@name", name.Trim());

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.Read() ? ReadProduct(reader) : null;
                    }
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public bool AddNewProductWithQuantity(string productName, int quantity)
        {
            const string query = @"
                INSERT INTO products (nameProduct, quantity)
                VALUES (@name, @quantity);
                SELECT LAST_INSERT_ID();";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@name", productName.Trim());
                    command.Parameters.AddWithValue("@quantity", quantity);

                    object result = command.ExecuteScalar();
                    LastInsertedId = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
                    return LastInsertedId > 0;
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public bool UpdateProduct(Products product)
        {
            const string query = @"
                UPDATE products
                SET nameProduct = @name,
                    quantity = @quantity
                WHERE idProducts = @id";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", product.IdProduct);
                    command.Parameters.AddWithValue("@name", product.Name.Trim());
                    command.Parameters.AddWithValue("@quantity", product.Quantity);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public void UpdateProductQuantity(int id, int quantity, bool isAddition)
        {
            string query = isAddition
                ? "UPDATE products SET quantity = quantity + @quantity WHERE idProducts = @id"
                : "UPDATE products SET quantity = quantity - @quantity WHERE idProducts = @id AND quantity >= @quantity";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@quantity", quantity);

                    if (command.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException("Не удалось обновить количество продукта.");
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public int GetMaxProductId()
        {
            const string query = "SELECT COALESCE(MAX(idProducts), 0) FROM products";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public List<Arrivals> GetAllArrivals()
        {
            const string query = @"
                SELECT idArrivals, date, productId, quantity
                FROM arrivals
                ORDER BY date DESC";

            List<Arrivals> arrivals = new List<Arrivals>();
            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        arrivals.Add(ReadArrival(reader));
                }

                return arrivals;
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public bool AddArrivals(Arrivals arrivals)
        {
            const string query = @"
                INSERT INTO arrivals (date, productId, quantity)
                VALUES (@date, @productId, @quantity)";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@date", arrivals.DateTime);
                    command.Parameters.AddWithValue("@productId", arrivals.ProductId);
                    command.Parameters.AddWithValue("@quantity", arrivals.Quantity);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public bool UpdateArrival(Arrivals arrival)
        {
            const string query = @"
                UPDATE arrivals
                SET date = @date,
                    productId = @productId,
                    quantity = @quantity
                WHERE idArrivals = @id";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", arrival.IdArrivals);
                    command.Parameters.AddWithValue("@date", arrival.DateTime);
                    command.Parameters.AddWithValue("@productId", arrival.ProductId);
                    command.Parameters.AddWithValue("@quantity", arrival.Quantity);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public List<Expenses> GetAllExpenses()
        {
            const string query = @"
                SELECT idExpenses, dateExpenses, productId, quantity
                FROM expenses
                ORDER BY dateExpenses DESC";

            List<Expenses> expenses = new List<Expenses>();
            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        expenses.Add(ReadExpense(reader));
                }

                return expenses;
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public bool AddExpenses(Expenses expenses)
        {
            const string query = @"
                INSERT INTO expenses (dateExpenses, productId, quantity)
                VALUES (@date, @productId, @quantity)";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@date", expenses.DateTime);
                    command.Parameters.AddWithValue("@productId", expenses.ProductId);
                    command.Parameters.AddWithValue("@quantity", expenses.Quantity);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public bool UpdateExpense(Expenses expense)
        {
            const string query = @"
                UPDATE expenses
                SET dateExpenses = @date,
                    productId = @productId,
                    quantity = @quantity
                WHERE idExpenses = @id";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", expense.IdExpenses);
                    command.Parameters.AddWithValue("@date", expense.DateTime);
                    command.Parameters.AddWithValue("@productId", expense.ProductId);
                    command.Parameters.AddWithValue("@quantity", expense.Quantity);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public int CountProductDocuments(int productId)
        {
            const string query = @"
                SELECT
                    (SELECT COUNT(*) FROM arrivals WHERE productId = @id) +
                    (SELECT COUNT(*) FROM expenses WHERE productId = @id)";

            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", productId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public bool DeleteProduct(Products product)
        {
            connect.OpenConnection();

            try
            {
                using (MySqlTransaction transaction = connect.GetConnection().BeginTransaction())
                {
                    ExecuteDelete("DELETE FROM arrivals WHERE productId = @id", product.IdProduct, transaction);
                    ExecuteDelete("DELETE FROM expenses WHERE productId = @id", product.IdProduct, transaction);
                    int deleted = ExecuteDelete("DELETE FROM products WHERE idProducts = @id", product.IdProduct, transaction);

                    transaction.Commit();
                    return deleted > 0;
                }
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public bool DeleteArrival(int arrivalId)
        {
            connect.OpenConnection();

            try
            {
                return ExecuteDelete("DELETE FROM arrivals WHERE idArrivals = @id", arrivalId, null) > 0;
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public bool DeleteExpense(int expenseId)
        {
            connect.OpenConnection();

            try
            {
                return ExecuteDelete("DELETE FROM expenses WHERE idExpenses = @id", expenseId, null) > 0;
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public List<Arrivals> GetArrivalsByDateRange(DateTime startDate, DateTime endDate)
        {
            const string query = @"
                SELECT idArrivals, date, productId, quantity
                FROM arrivals
                WHERE date >= @startDate AND date < @endDate
                ORDER BY date DESC";

            List<Arrivals> arrivals = new List<Arrivals>();
            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@startDate", startDate.Date);
                    command.Parameters.AddWithValue("@endDate", endDate.Date.AddDays(1));

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            arrivals.Add(ReadArrival(reader));
                    }
                }

                return arrivals;
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public List<Expenses> GetExpensesByDateRange(DateTime startDate, DateTime endDate)
        {
            const string query = @"
                SELECT idExpenses, dateExpenses, productId, quantity
                FROM expenses
                WHERE dateExpenses >= @startDate AND dateExpenses < @endDate
                ORDER BY dateExpenses DESC";

            List<Expenses> expenses = new List<Expenses>();
            connect.OpenConnection();

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@startDate", startDate.Date);
                    command.Parameters.AddWithValue("@endDate", endDate.Date.AddDays(1));

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            expenses.Add(ReadExpense(reader));
                    }
                }

                return expenses;
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        private int ExecuteDelete(string query, int id, MySqlTransaction transaction)
        {
            using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("@id", id);
                return command.ExecuteNonQuery();
            }
        }

        private static Products ReadProduct(MySqlDataReader reader)
        {
            return new Products
            {
                IdProduct = reader.GetInt32(reader.GetOrdinal("idProducts")),
                Name = reader.GetString(reader.GetOrdinal("nameProduct")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
            };
        }

        private static Arrivals ReadArrival(MySqlDataReader reader)
        {
            return new Arrivals
            {
                IdArrivals = reader.GetInt32(reader.GetOrdinal("idArrivals")),
                DateTime = reader.GetDateTime(reader.GetOrdinal("date")).Date,
                ProductId = reader.GetInt32(reader.GetOrdinal("productId")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
            };
        }

        private static Expenses ReadExpense(MySqlDataReader reader)
        {
            return new Expenses
            {
                IdExpenses = reader.GetInt32(reader.GetOrdinal("idExpenses")),
                DateTime = reader.GetDateTime(reader.GetOrdinal("dateExpenses")).Date,
                ProductId = reader.GetInt32(reader.GetOrdinal("productId")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
            };
        }
    }
}
