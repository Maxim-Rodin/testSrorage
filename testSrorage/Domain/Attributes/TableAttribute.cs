using System;

namespace testSrorage.Domain.Attributes
{
    // Атрибут для указания имени таблицы в БД для сущности.
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableAttribute : Attribute
    {
        public TableAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Table name cannot be empty.", nameof(name));

            Name = name;
        }

        public string Name { get; private set; }
    }
}
