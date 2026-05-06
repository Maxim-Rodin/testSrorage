using System;
using testSrorage.классы.интерфейсы;

namespace testSrorage.классы
{
    public abstract class DocumentBase : IDocument
    {
        public DateTime DateTime { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
