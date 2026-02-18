using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace testSrorage
{
    internal class DB
    {
        private MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=storage_db");

        public bool IsConnected => connection.State == ConnectionState.Open;

        public void OpenConnection()
        {
            if (connection.State == ConnectionState.Closed)
            {
                try
                {
                    connection.Open();
                }
                catch (MySqlException ex)
                {
                    
                    if (ex.Number == 1049) 
                    {
                        CreateDatabase();
                        connection.Open();
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка подключения: {ex.Message}");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения: {ex.Message}");
                    throw;
                }
            }
        }
        public void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public MySqlConnection GetConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                OpenConnection();
            }
            return connection;
        }
        private void CreateDatabase()
        {
            try
            {

                using (var tempConnection = new MySqlConnection("server=localhost;port=3306;username=root;password=root"))
                {
                    tempConnection.Open();


                    using (var cmd = tempConnection.CreateCommand())
                    {
                        cmd.CommandText = "CREATE DATABASE IF NOT EXISTS storage_db";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "USE storage_db";
                        cmd.ExecuteNonQuery();


                        CreateTables(cmd);
                    }
                }

                MessageBox.Show("База данных 'storage_db' создана автоматически!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания базы данных: {ex.Message}\n" +
                               "Убедитесь, что:\n" +
                               "1. Установлен MySQL Server\n" +
                               "2. Пользователь root имеет пароль root\n" +
                               "3. Сервер запущен");
                throw;
            }
        }
            public void CreateTables(MySqlCommand cmd)
            {
                
                cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS products (
                    idProducts INT PRIMARY KEY AUTO_INCREMENT,
                    nameProduct VARCHAR(100),
                    quantity INT DEFAULT 0
                )";
                cmd.ExecuteNonQuery();

                
                cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS arrivals (
                    idArrivals INT PRIMARY KEY AUTO_INCREMENT,
                    date DATETIME,
                    productId INT,
                    quantiti INT,
                    FOREIGN KEY (productId) REFERENCES products(idProducts)
                )";
                cmd.ExecuteNonQuery();

                
                cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS expenses (
                    idExpenses INT PRIMARY KEY AUTO_INCREMENT,
                    dateExpenses DATETIME,
                    ProductID INT,
                    quantityEx INT,
                    FOREIGN KEY (ProductID) REFERENCES products(idProducts)
                )";
                cmd.ExecuteNonQuery();

               
                cmd.CommandText = "SELECT COUNT(*) FROM products";
                var count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 0)
                {
                    
                    cmd.CommandText = @"
                    INSERT INTO products (nameProduct, quantity) VALUES
                    ('Арбуз', 10),
                    ('Банан', 25),
                    ('Яблоко', 30),
                    ('Груша', 15),
                    ('Апельсин', 20)";
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Добавлены тестовые данные в таблицу products");
                }
            }

        }
    }


