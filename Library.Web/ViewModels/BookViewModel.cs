using System.Collections.Generic;
using Library.Domain.Entities;

namespace Library.Web.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // This will store Author (Book), Publisher (Journal), or Student Name (Thesis)
        public string Author { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        // 🆕 Add this property to store the image filename or URL
        public string? ImageUrl { get; set; }

        public string ItemType { get; set; } = "Book";

        public bool IsAvailable { get; set; }
        public string? Edition { get; set; } // Can be Volume for Journals or Batch for Thesis
        public string PublicationYear { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        // Optional: Keep ISBN for Books, but it will be empty for Journals/Theses
        public string? ISBN { get; set; }
    }
}