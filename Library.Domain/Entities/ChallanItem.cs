using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class ChallanItem
    {
        public int ChallanItemId { get; set; }

        public int ChallanId { get; set; }
        public Challan Challan { get; set; }

        public string Particulars { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }
}
