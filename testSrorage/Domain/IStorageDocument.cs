using System;

namespace testSrorage.Domain
{
    // ќбщий интерфейс документа склада (приход/расход) Ч содержит дату, идентификатор продукта и количество.
    public interface IStorageDocument
    {
        DateTime DateTime { get; set; }
        int ProductId { get; set; }
        int Quantity { get; set; }
    }
}
