using System;

namespace testSrorage.классы.интерфейсы
{
    public interface IDocument
    {
        DateTime DateTime { get; set; }
        int ProductId { get; set; }
        int Quantity { get; set; }
    }
}
