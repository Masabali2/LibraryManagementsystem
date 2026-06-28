using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.DTOs
{
    public class UnifiedInventoryDto
    {
        // Common unique identifier
        public int Id { get; set; }

        // Mapped from Title (Book/Thesis) or JournalName (Journal)
        public string Title { get; set; } = null!;

        // Mapped from Author (Book), StudentName (Thesis), or Publisher (Journal)
        public string Author { get; set; } = null!;

        public string? Year { get; set; }

        // Mapped from TotalCopies (Book) or Quantity (Journal)
        public int TotalCopies { get; set; }

        // Mapped from AvailableCopies (Book) or Quantity (Journal)
        public int AvailableCopies { get; set; }

        public string? ImageUrl { get; set; }

        public string Department { get; set; } = null!;

        // "book", "journal", or "thesis" for UI logic
        public string Type { get; set; } = null!;

        // Physical Location Data
        public string ShelfCode { get; set; } = "N/A";
        public string BlockName { get; set; } = "Unassigned";
    }
}
