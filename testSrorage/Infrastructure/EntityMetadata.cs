using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using testSrorage.Domain;
using testSrorage.Domain.Attributes;

namespace testSrorage.Infrastructure
{
    internal static class EntityMetadata
    {
        public static string GetTableName(Type entityType)
        {
            TableAttribute attribute = entityType
                .GetCustomAttributes(typeof(TableAttribute), true)
                .Cast<TableAttribute>()
                .FirstOrDefault();

            return attribute == null ? entityType.Name : attribute.Name;
        }

        public static List<PropertyInfo> GetMappedProperties(Type entityType)
        {
            return entityType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead && property.CanWrite)
                .OrderBy(property => property.Name == nameof(BaseEntity.Id) ? 0 : 1)
                .ThenBy(property => property.Name)
                .ToList();
        }

        public static List<PropertyInfo> GetDataProperties(Type entityType)
        {
            return GetMappedProperties(entityType)
                .Where(property => property.Name != nameof(BaseEntity.Id))
                .ToList();
        }

        public static string EscapeIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Identifier cannot be empty.", nameof(identifier));

            return "`" + identifier.Replace("`", "``") + "`";
        }
    }
}
