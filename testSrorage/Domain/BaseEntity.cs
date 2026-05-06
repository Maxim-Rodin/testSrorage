namespace testSrorage.Domain
{
    // Базовый класс всех сущностей: содержит только Id.
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }
}
