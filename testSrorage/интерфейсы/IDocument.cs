using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testSrorage.классы.интерфейсы
{
    public interface IDocument
    {
        DateTime DateTime { get; set; }
        int ProductId { get; set; }
        int Quantity { get; set; }

      
    }
}
