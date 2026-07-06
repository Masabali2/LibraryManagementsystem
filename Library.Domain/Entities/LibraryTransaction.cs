using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class LibraryTransaction
    {
        public int LibraryTransactionId { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int? ItemId { get; set; }

        public string ItemTitle { get; set; } = string.Empty;
        public string ItemType { get; set; } = "Book";

        public string TransactionType { get; set; } = "Borrow"; // Borrow / Sell

        public DateTime IssueDate { get; set; } = DateTime.Today;
        public DateTime? DueDate { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal Price { get; set; }
        public decimal FineAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public bool GenerateChallan { get; set; }
        public bool NotifyStudent { get; set; }

        public int? ChallanId { get; set; }

        public string Status { get; set; } = "Active";
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
