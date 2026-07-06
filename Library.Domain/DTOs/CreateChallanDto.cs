using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.DTOs
{

    public class CreateChallanDto
    {
        public int StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;
        public string RollNo { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; } = DateTime.Today;
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        public string BankName { get; set; } = "Bank of Punjab";
        public string AccountTitle { get; set; } = "USKT Library";
        public string AccountNo { get; set; } = string.Empty;

        public int BorrowedBooksCount { get; set; }
        public int IssuedBooksCount { get; set; }
        public int PurchasedBooksCount { get; set; }

        public string Status { get; set; } = "Unpaid";
        public string? Notes { get; set; }

        public decimal TotalAmount { get; set; }

        public List<CreateChallanItemDto> Items { get; set; } = new();
    }

    public class CreateChallanItemDto
    {
        public string Particulars { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }
}
