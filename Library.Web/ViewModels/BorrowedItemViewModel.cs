using System.Collections.Generic;
using Library.Domain.Entities;

namespace Library.Web.ViewModels
{
    public class BorrowedItemViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty; // e.g., Book, Thesis, Journal
        public DateTime BorrowedDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsDueSoon { get; set; }
    }
}
