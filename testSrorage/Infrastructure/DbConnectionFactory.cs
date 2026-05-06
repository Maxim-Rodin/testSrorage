using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace testSrorage.Infrastructure
{
    // Фабрика подключений к MySQL, предоставляет строки подключения и объекты MySqlConnection.
    public sealed class DbConnectionFactory
    {
        private const string DefaultConnectionString =
            "server=localhost;port=3306;username=root;password=Xameleon8805;database=storage_db";

        private readonly string connectionString;

        // Конструктор по умолчанию использует конфигурацию приложения или значение по умолчанию.
        public DbConnectionFactory()
            : this(GetConfiguredConnectionString())
        {
        }

        // Конструктор с явной строкой подключения.
        public DbConnectionFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));

            this.connectionString = connectionString;
        }

        // Возвращает имя базы данных из строки подключения.
        public string DatabaseName
        {
            get
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);
                return builder.Database;
            }
        }

        // Создаёт подключение к конкретной базе данных.
        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // Создаёт подключение к серверу без указания базы (для создания базы и т.п.).
        public MySqlConnection CreateServerConnection()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);
            builder.Database = string.Empty;
            return new MySqlConnection(builder.ConnectionString);
        }

        // Получает строку подключения из конфигурации или возвращает значение по умолчанию.
        private static string GetConfiguredConnectionString()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["StorageDb"];
            return settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString)
                ? DefaultConnectionString
                : settings.ConnectionString;
        }
    }
}
