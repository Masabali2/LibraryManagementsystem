using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.DTOs
{
    public class DirectBorrowDto
    {
        // Student information (auto-filled, read-only in the form)
        public int StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string RollNo { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Batch { get; set; } = string.Empty;

        // Item / book information
        [Required(ErrorMessage = "Please select an item.")]
        public int ItemId { get; set; }

        public string ItemTitle { get; set; } = string.Empty;

        public string ItemType { get; set; } = "Book";

        // Borrow or Sell
        [Required(ErrorMessage = "Please select transaction type.")]
        public string TransactionType { get; set; } = "Borrow";

        // Dates
        [Required(ErrorMessage = "Issue date is required.")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; } = DateTime.Today.AddDays(14);

        // Financial information
        [Range(1, 100, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        [Range(0, 999999, ErrorMessage = "Price cannot be negative.")]
        public decimal Price { get; set; } = 0;

        [Range(0, 999999, ErrorMessage = "Fine amount cannot be negative.")]
        public decimal FineAmount { get; set; } = 0;

        public decimal TotalAmount { get; set; }

        // Optional actions
        public bool GenerateChallan { get; set; }

        public bool NotifyStudent { get; set; }

        public string? Notes { get; set; }
        public List<LibraryItemOptionDto> AvailableItems { get; set; } = new();
    }
    public class LibraryItemOptionDto
    {
        public int ItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ItemType { get; set; } = "Book";
    }
}
