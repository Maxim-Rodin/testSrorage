using System;
using testSrorage.Domain.Attributes;

namespace testSrorage.Domain
{
    [Table("expenses")]
    public class Expense : BaseEntity, IStorageDocument, IOutgoingDocument
    {
        public DateTime DateTime { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
