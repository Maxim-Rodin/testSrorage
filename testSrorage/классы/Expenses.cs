using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testSrorage.классы.интерфейсы;

namespace testSrorage.классы
{
    public class Expenses : IDocument 
    {
        public int IdExpenses { get; set; }
        public DateTime DateTime { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
