using System;
using testSrorage.Domain.Attributes;

namespace testSrorage.Domain
{
    [Table("arrivals")]
    public class Arrival : BaseEntity, IStorageDocument, IIncomingDocument
    {
        public DateTime DateTime { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
