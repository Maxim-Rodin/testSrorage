using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using testSrorage.Domain;
using testSrorage.Domain.Attributes;

namespace testSrorage.Infrastructure
{
    // Вспомогательный класс, собирающий метаданные сущностей (имена таблиц, свойства и т.д.).
    internal static class EntityMetadata
    {
        // Возвращает имя таблицы для типа сущности (атрибут Table или имя типа).
        public static string GetTableName(Type entityType)
        {
            TableAttribute attribute = entityType
                .GetCustomAttributes(typeof(TableAttribute), true)
                .Cast<TableAttribute>()
                .FirstOrDefault();

            return attribute == null ? entityType.Name : attribute.Name;
        }

        // Возвращает список свойств сущности для маппинга (Id первым).
        public static List<PropertyInfo> GetMappedProperties(Type entityType)
        {
            return entityType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead && property.CanWrite)
                .OrderBy(property => property.Name == nameof(BaseEntity.Id) ? 0 : 1)
                .ThenBy(property => property.Name)
                .ToList();
        }

        // Возвращает свойства, которые участвуют в INSERT/UPDATE (без Id).
        public static List<PropertyInfo> GetDataProperties(Type entityType)
        {
            return GetMappedProperties(entityType)
                .Where(property => property.Name != nameof(BaseEntity.Id))
                .ToList();
        }

        // Экранирует идентификатор (имя столбца/таблицы) для SQL-запросов.
        public static string EscapeIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Identifier cannot be empty.", nameof(identifier));

            return "`" + identifier.Replace("`", "``") + "`";
        }
    }
}
