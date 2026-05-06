using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testSrorage.инфраструктура
{
    internal class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS products (
                idProducts INT AUTO_INCREMENT PRIMARY KEY,
                nameProduct VARCHAR(100),
                quantity INT
            );";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS arrivals (
                idArrivals INT AUTO_INCREMENT PRIMARY KEY,
                date DATETIME,
                productId INT,
                quantity INT
            );";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS expenses (
                idExpenses INT AUTO_INCREMENT PRIMARY KEY,
                dateExpenses DATETIME,
                productId INT,
                quantity INT
            );";
            cmd.ExecuteNonQuery();
        }

    }
}
