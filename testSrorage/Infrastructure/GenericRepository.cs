using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;

namespace testSrorage.Infrastructure
{
    // Универсальный репозиторий, реализующий CRUD для сущности T через MySQL.
    public sealed class GenericRepository<T> : IRepository<T>
        where T : BaseEntity, new()
    {
        private readonly DbConnectionFactory connectionFactory;
        private readonly string tableName;
        private readonly List<PropertyInfo> mappedProperties;
        private readonly List<PropertyInfo> dataProperties;

        // Конструктор — получает фабрику подключений и готовит метаданные сущности.
        public GenericRepository(DbConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            this.connectionFactory = connectionFactory;
            tableName = EntityMetadata.GetTableName(typeof(T));
            mappedProperties = EntityMetadata.GetMappedProperties(typeof(T));
            dataProperties = EntityMetadata.GetDataProperties(typeof(T));
        }

        // Возвращает все записи таблицы.
        public List<T> GetAll()
        {
            string sql = "SELECT " + BuildColumnList(mappedProperties) +
                         " FROM " + EntityMetadata.EscapeIdentifier(tableName);

            List<T> items = new List<T>();

            using (MySqlConnection connection = connectionFactory.CreateConnection())
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        items.Add(Map(reader));
                }
            }

            return items;
        }

        // Возвращает запись по Id.
        public T GetById(int id)
        {
            string sql = "SELECT " + BuildColumnList(mappedProperties) +
                         " FROM " + EntityMetadata.EscapeIdentifier(tableName) +
                         " WHERE " + EntityMetadata.EscapeIdentifier(nameof(BaseEntity.Id)) + " = @Id LIMIT 1";

            using (MySqlConnection connection = connectionFactory.CreateConnection())
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? Map(reader) : null;
                }
            }
        }

        // Вставляет запись и возвращает её новый Id.
        public int Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            string columns = string.Join(", ", dataProperties.Select(property => EntityMetadata.EscapeIdentifier(property.Name)));
            string parameters = string.Join(", ", dataProperties.Select(property => "@" + property.Name));
            string sql = "INSERT INTO " + EntityMetadata.EscapeIdentifier(tableName) +
                         " (" + columns + ") VALUES (" + parameters + "); SELECT LAST_INSERT_ID();";

            using (MySqlConnection connection = connectionFactory.CreateConnection())
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                AddParameters(command, entity, dataProperties);
                connection.Open();

                object result = command.ExecuteScalar();
                entity.Id = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
                return entity.Id;
            }
        }

        // Обновляет существующую запись.
        public bool Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            string setClause = string.Join(
                ", ",
                dataProperties.Select(property => EntityMetadata.EscapeIdentifier(property.Name) + " = @" + property.Name));

            string sql = "UPDATE " + EntityMetadata.EscapeIdentifier(tableName) +
                         " SET " + setClause +
                         " WHERE " + EntityMetadata.EscapeIdentifier(nameof(BaseEntity.Id)) + " = @Id";

            using (MySqlConnection connection = connectionFactory.CreateConnection())
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                AddParameters(command, entity, dataProperties);
                command.Parameters.AddWithValue("@Id", entity.Id);
                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }

        // Удаляет запись по Id.
        public bool Delete(int id)
        {
            string sql = "DELETE FROM " + EntityMetadata.EscapeIdentifier(tableName) +
                         " WHERE " + EntityMetadata.EscapeIdentifier(nameof(BaseEntity.Id)) + " = @Id";

            using (MySqlConnection connection = connectionFactory.CreateConnection())
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }

        // Вспомогательный: строит список колонок для SELECT.
        private static string BuildColumnList(IEnumerable<PropertyInfo> properties)
        {
            return string.Join(", ", properties.Select(property => EntityMetadata.EscapeIdentifier(property.Name)));
        }

        // Вспомогательный: добавляет параметры команды из свойств сущности.
        private static void AddParameters(MySqlCommand command, T entity, IEnumerable<PropertyInfo> properties)
        {
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(entity, null);
                command.Parameters.AddWithValue("@" + property.Name, value ?? DBNull.Value);
            }
        }

        // Маппит строку результата в объект T.
        private T Map(IDataRecord record)
        {
            T entity = new T();

            foreach (PropertyInfo property in mappedProperties)
            {
                int ordinal = record.GetOrdinal(property.Name);
                if (record.IsDBNull(ordinal))
                    continue;

                object value = record.GetValue(ordinal);
                Type targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                if (targetType.IsEnum)
                    property.SetValue(entity, Enum.ToObject(targetType, value), null);
                else
                    property.SetValue(entity, Convert.ChangeType(value, targetType), null);
            }

            return entity;
        }
    }
}
