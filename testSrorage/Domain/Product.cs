using testSrorage.Domain.Attributes;

namespace testSrorage.Domain
{
    // —ущность продукта на складе Ч хранит им€ и текущее количество.
    [Table("products")]
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
