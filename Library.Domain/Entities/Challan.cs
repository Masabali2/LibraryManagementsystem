using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class Challan
    {
        public int ChallanId { get; set; }
        public string ChallanNo { get; set; } = string.Empty;

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }

        public string BankName { get; set; } = "Bank of Punjab";
        public string AccountTitle { get; set; } = "USKT Library";
        public string AccountNo { get; set; } = "03167185223";

        public int BorrowedBooksCount { get; set; }
        public int IssuedBooksCount { get; set; }
        public int PurchasedBooksCount { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Unpaid";
        public string? Notes { get; set; }

        public ICollection<ChallanItem> Items { get; set; } = new List<ChallanItem>();
    }
}
