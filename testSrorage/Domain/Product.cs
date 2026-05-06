using testSrorage.Domain.Attributes;

namespace testSrorage.Domain
{
    [Table("products")]
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
