using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testSrorage.классы.интерфейсы
{
    internal interface IDocument
    {
        DateTime DateTime { get; set; }

        int ProductId { get; set; }
        int Quantiti { get; set; }
    }
}
