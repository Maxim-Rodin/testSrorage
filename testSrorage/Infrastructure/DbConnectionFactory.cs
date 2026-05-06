using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace testSrorage.Infrastructure
{
    public sealed class DbConnectionFactory
    {
        private const string DefaultConnectionString =
            "server=localhost;port=3306;username=root;password=Xameleon8805;database=storage_db";

        private readonly string connectionString;

        public DbConnectionFactory()
            : this(GetConfiguredConnectionString())
        {
        }

        public DbConnectionFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));

            this.connectionString = connectionString;
        }

        public string DatabaseName
        {
            get
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);
                return builder.Database;
            }
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public MySqlConnection CreateServerConnection()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);
            builder.Database = string.Empty;
            return new MySqlConnection(builder.ConnectionString);
        }

        private static string GetConfiguredConnectionString()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["StorageDb"];
            return settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString)
                ? DefaultConnectionString
                : settings.ConnectionString;
        }
    }
}
