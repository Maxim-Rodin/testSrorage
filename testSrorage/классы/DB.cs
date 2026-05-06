using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace testSrorage
{
    internal sealed class DB : IDisposable
    {
        private const string DefaultConnectionString =
            "server=localhost;port=3306;username=root;password=Xameleon8805;database=storage_db";

        private readonly MySqlConnection connection;

        public DB()
            : this(GetConfiguredConnectionString())
        {
        }

        public DB(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Строка подключения не может быть пустой.", nameof(connectionString));

            connection = new MySqlConnection(connectionString);
        }

        public bool IsConnected
        {
            get { return connection.State == ConnectionState.Open; }
        }

        public void OpenConnection()
        {
            if (connection.State != ConnectionState.Closed)
                return;

            try
            {
                connection.Open();
            }
            catch (MySqlException ex)
            {
                if (ex.Number != 1049)
                    throw new InvalidOperationException("Ошибка подключения к базе данных: " + ex.Message, ex);

                CreateDatabase();
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        public MySqlConnection GetConnection()
        {
            if (connection.State != ConnectionState.Open)
                OpenConnection();

            return connection;
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        private static string GetConfiguredConnectionString()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["StorageDb"];
            return settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString)
                ? DefaultConnectionString
                : settings.ConnectionString;
        }

        private void CreateDatabase()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connection.ConnectionString);
            string databaseName = builder.Database;
            builder.Database = string.Empty;

            using (MySqlConnection temp = new MySqlConnection(builder.ConnectionString))
            using (MySqlCommand cmd = temp.CreateCommand())
            {
                temp.Open();
                cmd.CommandText = "CREATE DATABASE IF NOT EXISTS `" + EscapeIdentifier(databaseName) + "`";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "USE `" + EscapeIdentifier(databaseName) + "`";
                cmd.ExecuteNonQuery();

                CreateTables(cmd);
            }
        }

        private static string EscapeIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new InvalidOperationException("В строке подключения не указано имя базы данных.");

            return identifier.Replace("`", "``");
        }

        private static void CreateTables(MySqlCommand cmd)
        {
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS products (
                    idProducts INT PRIMARY KEY AUTO_INCREMENT,
                    nameProduct VARCHAR(100) NOT NULL,
                    quantity INT NOT NULL DEFAULT 0
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS arrivals (
                    idArrivals INT PRIMARY KEY AUTO_INCREMENT,
                    date DATETIME NOT NULL,
                    productId INT NOT NULL,
                    quantity INT NOT NULL,
                    FOREIGN KEY (productId) REFERENCES products(idProducts)
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS expenses (
                    idExpenses INT PRIMARY KEY AUTO_INCREMENT,
                    dateExpenses DATETIME NOT NULL,
                    productId INT NOT NULL,
                    quantity INT NOT NULL,
                    FOREIGN KEY (productId) REFERENCES products(idProducts)
                )";
            cmd.ExecuteNonQuery();
        }
    }
}
