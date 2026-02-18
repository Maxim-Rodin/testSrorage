using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using testSrorage.классы;
using testSrorage.классы.интерфейсы;

namespace testSrorage
{
    internal class DBManager
    {
        private DB connect = new DB();
        public int LastInsertedId { get; set; }

        #region Методы для продуктов

        public List<Products> GetAllProducts() // получения списка из всех продуктов
        {
            List<Products> products = new List<Products>();
            string query = "SELECT * FROM products";

            try
            {
                connect.OpenConnection();
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Products product = new Products
                        {
                            IdProduct = reader.GetInt32("idProducts"),
                            Name = reader.GetString("nameProduct"),
                            Quantity = reader.GetInt32("quantity")
                        };
                        products.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении продуктов: {ex.Message}");
            }
            finally
            {
                connect.CloseConnection();
            }

            return products;
        }
        public bool UpdateProduct(Products product)
        {
            try
            {
                connect.OpenConnection();

                string query = @"
                UPDATE products 
                SET nameProduct = @name, 
                    quantity = @quantity 
                WHERE idProducts = @id";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", product.IdProduct);
                    command.Parameters.AddWithValue("@name", product.Name);
                    command.Parameters.AddWithValue("@quantity", product.Quantity);

                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении продукта: {ex.Message}");
                return false;
            }
            finally
            {
                connect.CloseConnection();
            }
        }
        public Products GetProductById(int id) // метод для нахождения продукта по id
        {
            try
            {
                connect.OpenConnection();
                string query = "SELECT * FROM products WHERE idProducts = @id";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Products
                            {
                                IdProduct = reader.GetInt32("idProducts"),
                                Name = reader.GetString("nameProduct"),
                                Quantity = reader.GetInt32("quantity")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске продукта по ID: {ex.Message}");
            }
            finally
            {
                connect.CloseConnection();
            }

            return null;
        }
        public bool AddNewProductWithQuantity(string productName, int quantity) // добавление нового продукта
        {
            try
            {
                connect.OpenConnection();


                string query = "INSERT INTO products (nameProduct, quantity) VALUES (@name, @quantity); SELECT LAST_INSERT_ID();";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@name", productName);
                    command.Parameters.AddWithValue("@quantity", quantity);

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        LastInsertedId = Convert.ToInt32(result);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании продукта: {ex.Message}");
                return false;
            }
            finally
            {
                connect.CloseConnection();
            }
        }
        public int GetMaxProductId() // получение id для нового продукта
        {
            try
            {
                connect.OpenConnection();
                string query = "SELECT MAX(idProducts) FROM products";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении максимального ID: {ex.Message}");
                return 0;
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public void UpdateProductQuantity(int id, int quantity, bool isAddition) // обновление колличества продуктов на складе 
        {
            try
            {
                connect.OpenConnection();
                string operation = isAddition ? "+" : "-";
                string query = $"UPDATE products SET quantity = quantity {operation} @quantity WHERE idProducts = @id";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении количества: {ex.Message}");
                throw;
            }
            finally
            {
                connect.CloseConnection();
            }
        }
        public Products GetProductByName(string name) // нахождение продукта по имени
        {
            try
            {
                connect.OpenConnection();
                string query = "SELECT * FROM products WHERE nameProduct = @name";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@name", name);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Products
                            {
                                IdProduct = reader.GetInt32("idProducts"),
                                Name = reader.GetString("nameProduct"),
                                Quantity = reader.GetInt32("quantity")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске продукта: {ex.Message}");
            }
            finally
            {
                connect.CloseConnection();
            }

            return null;
        }


        #endregion


        #region Методы для приходов
        public List<Arrivals> GetAllArrivals()
        {
            List<Arrivals> arrivals = new List<Arrivals>();
            string query = "SELECT * FROM arrivals";
            try
            {
                connect.OpenConnection();

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Arrivals arrival = new Arrivals
                            {
                                IdArrivals = reader.GetInt32("idArrivals"),
                                ProductId = reader.GetInt32("productId"),
                                DateTime = reader.GetDateTime("date").Date,
                                Quantiti = reader.GetInt32("quantiti")



                            };
                            arrivals.Add(arrival);
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении приходов: {ex.Message}");
            }
            finally
            {
                connect.CloseConnection();
            }

            return arrivals;
        }
        public bool UpdateArrival(Arrivals arrival)
        {
            try
            {
                connect.OpenConnection();

                string query = @"
                UPDATE arrivals 
                SET date = @date, 
                    productId = @productId, 
                    quantiti = @quantiti 
                WHERE idArrivals = @id";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", arrival.IdArrivals);
                    command.Parameters.AddWithValue("@date", arrival.DateTime);
                    command.Parameters.AddWithValue("@productId", arrival.ProductId);
                    command.Parameters.AddWithValue("@quantiti", arrival.Quantiti);

                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении прихода: {ex.Message}");
                return false;
            }
            finally
            {
                connect.CloseConnection();
            }
        }
        public bool AddArrivals(Arrivals arrivals) // метод для добавления прихода
        {
            try
            {
                connect.OpenConnection();


                int nextArrivalId = GetNextArrivalId();


                string query = @"INSERT INTO arrivals (idArrivals, date, productId, quantiti) 
                         VALUES (@id, @date, @productId, @quantiti)";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", nextArrivalId);
                    command.Parameters.AddWithValue("@date", arrivals.DateTime);
                    command.Parameters.AddWithValue("@productId", arrivals.ProductId);
                    command.Parameters.AddWithValue("@quantiti", arrivals.Quantiti);

                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении прихода: {ex.Message}");
                return false;
            }
            finally
            {
                connect.CloseConnection();
            }
        }
        private bool IsArrivalIdExists(int id) // проверяем найден ли приход по айди
        {
            string query = "SELECT 1 FROM arrivals WHERE idArrivals = @id LIMIT 1";

            using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
            {
                command.Parameters.AddWithValue("@id", id);
                object result = command.ExecuteScalar();
                return result != null;
            }
        }
        private int GetNextArrivalId() // получение id
        {
            try
            {
                string query = "SELECT COALESCE(MAX(idArrivals), 0) FROM arrivals";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    object result = command.ExecuteScalar();
                    int maxId = Convert.ToInt32(result);


                    int nextId = maxId + 1;
                    while (IsArrivalIdExists(nextId))
                    {
                        nextId++;
                    }

                    return nextId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении ID прихода: {ex.Message}");
                return 1;
            }
        }
        #endregion


        #region Методы для расходов
        public List<Expenses> GetAllExpenses()
        {
            List<Expenses> expenseses = new List<Expenses>();
            string query = "SELECT * FROM expenses";

            try
            {
               connect.OpenConnection();
               
                using (MySqlCommand command = new MySqlCommand(query,connect.GetConnection()))
                {
                    using (MySqlDataReader reader = command.ExecuteReader()) {
                        
                       while (reader.Read())
                        {

                            Expenses expenses = new Expenses
                            {
                                IdExpenses = reader.GetInt32("idExpenses"),
                                ProductId =reader.GetInt32("ProductID"),
                                DateTime = reader.GetDateTime("dateExpenses").Date,
                                Quantiti = reader.GetInt32("quantityEx")
                                
                            };
                            expenseses.Add(expenses);
                        }
                    
                    
                    }


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении расходов: {ex.Message}");
            }
            finally
            {
                connect.CloseConnection();
            }

            return expenseses;

        }
        public bool UpdateExpense(Expenses expense)
        {
            try
            {
                connect.OpenConnection();

                string query = @"
                UPDATE expenses 
                SET dateExpenses = @date, 
                    ProductID = @productId, 
                    quantityEx = @quantity 
                WHERE idExpenses = @id";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", expense.IdExpenses);
                    command.Parameters.AddWithValue("@date", expense.DateTime);
                    command.Parameters.AddWithValue("@productId", expense.ProductId);
                    command.Parameters.AddWithValue("@quantity", expense.Quantiti);

                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении расхода: {ex.Message}");
                return false;
            }
            finally
            {
                connect.CloseConnection();
            }
        }
        private bool IsEpenesIdExists(int id) // проверяем найден ли расход по айди
        {
            string query = "SELECT 1 FROM expenses WHERE IdExpenses = @id LIMIT 1";

            using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
            {
                command.Parameters.AddWithValue("@id", id);
                object result = command.ExecuteScalar();
                return result != null;
            }
        }

        public int GetNextExpenses ()
        {
            try
            {
                string query = "SELECT COALESCE(MAX(IdExpenses), 0) FROM expenses";
                using (MySqlCommand command = new MySqlCommand(query ,connect.GetConnection()))
                {
                    object result = command.ExecuteScalar();
                    int maxId = Convert.ToInt32(result);


                    int nextId = maxId + 1;
                    while (IsEpenesIdExists(nextId))
                    {
                        nextId++;
                    }

                    return nextId;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Ошибка при получении ID расход: {ex.Message}");
                return 1;
            }
            
        }
        public bool AddExpenses (Expenses expenses)
        {
            try
            {
                connect.OpenConnection();

                int nextExpensesId = GetNextExpenses();
                string query = @"INSERT INTO expenses (idExpenses, dateExpenses, ProductID, quantityEx) 
                         VALUES (@id, @date, @productId, @quantiti)";
                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", nextExpensesId);
                    command.Parameters.AddWithValue("@date", expenses.DateTime);
                    command.Parameters.AddWithValue("@productId",expenses.ProductId);
                    command.Parameters.AddWithValue("@quantiti", expenses.Quantiti);

                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows > 0;
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении расхода: {ex.Message}");
                return false;
            }
            finally { connect.CloseConnection(); }
        }
        #endregion

        #region Методы удаления
        public bool DeleteProduct(Products product)
        {
            try
            {
                connect.OpenConnection();

                
                string query = @"SELECT COUNT(*) FROM arrivals WHERE productId = @id  
                         UNION ALL    
                         SELECT COUNT(*) FROM expenses WHERE ProductID = @id";

                int totalReload = 0;
                using (MySqlCommand commandCheck = new MySqlCommand(query, connect.GetConnection()))
                {
                    commandCheck.Parameters.AddWithValue("@id", product.IdProduct); 

                    using (MySqlDataReader reader = commandCheck.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            totalReload += reader.GetInt32(0);
                        }
                    }

                    
                    if (totalReload > 0)
                    {
                        MessageBoxResult result = MessageBox.Show(
                            $"У продукта есть {totalReload} связанных записей (приходы/расходы).\n" +
                            "Удалить продукт и все связанные записи?",
                            "Подтверждение удаления",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result != MessageBoxResult.Yes)
                        {
                            return false; 
                        }

                       
                        string deleteRelatedQuery = @"
                    DELETE FROM arrivals WHERE productId = @id;
                    DELETE FROM expenses WHERE ProductID = @id;";

                        using (MySqlCommand commandDelete = new MySqlCommand(deleteRelatedQuery, connect.GetConnection()))
                        {
                            commandDelete.Parameters.AddWithValue("@id", product.IdProduct);
                            commandDelete.ExecuteNonQuery();
                        }
                    }

                    
                    string deleteProductQuery = "DELETE FROM products WHERE idProducts = @id";
                    using (MySqlCommand command = new MySqlCommand(deleteProductQuery, connect.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@id", product.IdProduct);
                        int affectedRows = command.ExecuteNonQuery();
                        MessageBox.Show($"Удален {product.Name}");
                        return affectedRows > 0;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления  {ex.Message}");
                return false;
            }
            finally
            {
                connect.CloseConnection();
            }
        }
        public bool DeleteArrival(int arrivalId)
        {
            try
            {
                connect.OpenConnection();

                string query = @"
                DELETE FROM arrivals 
                WHERE idArrivals = @id";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", arrivalId);
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении прихода: {ex.Message}");
                return false;
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        public bool DeleteExpense(int expenseId)
        {
            try
            {
                connect.OpenConnection();

                string query = @"
                DELETE FROM expenses 
                WHERE idExpenses = @id";

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@id", expenseId);
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении расхода: {ex.Message}");
                return false;
            }
            finally
            {
                connect.CloseConnection();
            }
        }

        #endregion


        #region Методы для отчетов

        public List<Arrivals> GetArrivalsByDateRange(DateTime startDate, DateTime endDate)
        {
            List<Arrivals> arrivals = new List<Arrivals>();

            
            MessageBox.Show($"SQL запрос: startDate={startDate:yyyy-MM-dd}, endDate={endDate:yyyy-MM-dd}");

         
            string query = @"
SELECT 
    a.idArrivals,
    a.date,
    a.productId,
    a.quantiti,
    DATE(a.date) as DateOnly
FROM arrivals a
WHERE DATE(a.date) >= DATE(@startDate) 
  AND DATE(a.date) <= DATE(@endDate)
ORDER BY a.date DESC";

            try
            {
                connect.OpenConnection();

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                   
                    command.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                   
                    MessageBox.Show($"Выполняем запрос: {command.CommandText}");

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            count++;
                            arrivals.Add(new Arrivals
                            {
                                IdArrivals = reader.GetInt32("idArrivals"),
                                DateTime = reader.GetDateTime("date"),
                                ProductId = reader.GetInt32("productId"),
                                Quantiti = reader.GetInt32("quantiti")
                            });
                        }
                        MessageBox.Show($"Найдено записей: {count}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении приходов: {ex.Message}\n\nЗапрос: {query}");
            }
            finally
            {
                connect.CloseConnection();
            }

            return arrivals;
        }
        public List<Expenses> GetExpensesByDateRange(DateTime startDate, DateTime endDate)
        {
            List<Expenses> expenses = new List<Expenses>();

            string query = @"
            SELECT e.*, p.nameProduct 
            FROM expenses e
            LEFT JOIN products p ON e.ProductID = p.idProducts
            WHERE e.dateExpenses BETWEEN @startDate AND @endDate 
            ORDER BY e.dateExpenses DESC";

            try
            {
                connect.OpenConnection();

                using (MySqlCommand command = new MySqlCommand(query, connect.GetConnection()))
                {
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate.AddDays(1).AddSeconds(-1));

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            expenses.Add(new Expenses
                            {
                                IdExpenses = reader.GetInt32("idExpenses"),
                                DateTime = reader.GetDateTime("dateExpenses"),
                                ProductId = reader.GetInt32("ProductID"),
                                Quantiti = reader.GetInt32("quantityEx"),
                                
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении расходов: {ex.Message}");
            }
            finally
            {
                connect.CloseConnection();
            }

            return expenses;
        }

        

        #endregion














    }


}
