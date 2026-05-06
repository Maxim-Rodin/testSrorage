using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using testSrorage.Domain;
using testSrorage.Domain.Attributes;

namespace testSrorage.Infrastructure
{
    public sealed class DatabaseInitializer
    {
        private readonly DbConnectionFactory connectionFactory;

        public DatabaseInitializer(DbConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            this.connectionFactory = connectionFactory;
        }

        public void Initialize(params Assembly[] assemblies)
        {
            EnsureDatabaseExists();

            List<Type> entityTypes = GetEntityTypes(assemblies);
            using (MySqlConnection connection = connectionFactory.CreateConnection())
            {
                connection.Open();

                foreach (Type entityType in entityTypes)
                    EnsureTableExists(connection, entityType);
            }
        }

        private void EnsureDatabaseExists()
        {
            string databaseName = connectionFactory.DatabaseName;
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("В строке подключения не указано имя базы данных.");

            string sql = "CREATE DATABASE IF NOT EXISTS " +
                         EntityMetadata.EscapeIdentifier(databaseName) +
                         " CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";

            using (MySqlConnection connection = connectionFactory.CreateServerConnection())
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static List<Type> GetEntityTypes(IEnumerable<Assembly> assemblies)
        {
            IEnumerable<Assembly> source = assemblies == null || !assemblies.Any()
                ? new[] { typeof(BaseEntity).Assembly }
                : assemblies;

            return source
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => typeof(BaseEntity).IsAssignableFrom(type))
                .Where(type => type.GetCustomAttributes(typeof(TableAttribute), true).Any())
                .OrderBy(type => type.Name)
                .ToList();
        }

        private static void EnsureTableExists(MySqlConnection connection, Type entityType)
        {
            string tableName = EntityMetadata.GetTableName(entityType);
            List<PropertyInfo> properties = EntityMetadata.GetMappedProperties(entityType);
            string columns = string.Join(", ", properties.Select(CreateColumnDefinition));

            string sql = "CREATE TABLE IF NOT EXISTS " +
                         EntityMetadata.EscapeIdentifier(tableName) +
                         " (" + columns + ")";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static string CreateColumnDefinition(PropertyInfo property)
        {
            if (property.Name == nameof(BaseEntity.Id))
            {
                return EntityMetadata.EscapeIdentifier(property.Name) +
                       " INT NOT NULL AUTO_INCREMENT PRIMARY KEY";
            }

            return EntityMetadata.EscapeIdentifier(property.Name) +
                   " " + GetSqlType(property.PropertyType) +
                   " " + GetNullability(property.PropertyType);
        }

        private static string GetSqlType(Type propertyType)
        {
            Type type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (type == typeof(string))
                return "VARCHAR(255)";
            if (type == typeof(int))
                return "INT";
            if (type == typeof(long))
                return "BIGINT";
            if (type == typeof(decimal))
                return "DECIMAL(18,2)";
            if (type == typeof(double) || type == typeof(float))
                return "DOUBLE";
            if (type == typeof(bool))
                return "TINYINT(1)";
            if (type == typeof(DateTime))
                return "DATETIME";

            throw new NotSupportedException("Тип свойства не поддерживается ORM-инициализатором: " + type.FullName);
        }

        private static string GetNullability(Type propertyType)
        {
            if (!propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) != null)
                return "NULL";

            return "NOT NULL";
        }
    }
}
