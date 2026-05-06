using System;

namespace testSrorage.классы
{
    public abstract class Document
    {
        public int Id { get; private set; }
        public DateTime Date { get; private set; }
        public int ProductId { get; protected set; }
        public int Quantity { get; protected set; }

        protected Document(int productId, int quantity, DateTime date)
        {
            ProductId = productId;
            Quantity = quantity;
            Date = date;
        }

        protected Document(int productId, int quantity)
            : this(productId, quantity, DateTime.Now)
        {
        }

        public void SetId(int id)
        {
            Id = id;
        }

        // оставляем только для БД (не для бизнес-логики)
        public void SetDate(DateTime date)
        {
            Date = date;
        }
    }
}