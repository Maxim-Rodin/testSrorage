using System;

namespace testSrorage.Domain
{
    public interface IStorageDocument
    {
        DateTime DateTime { get; set; }
        int ProductId { get; set; }
        int Quantity { get; set; }
    }
}
