using Library.Domain.Entities;
using System.Collections.Generic;

namespace Library.Web.ViewModels
{
    public class InventoryIndexViewModel
    {
        // Lists to hold the data for the three categories
        public List<Book> Books { get; set; } = new List<Book>();
        public List<Thesis> Theses { get; set; } = new List<Thesis>();
        public List<Journal> Journals { get; set; } = new List<Journal>();

        // Optional: Summary properties for the top stats
        public int TotalAssets => Books.Count + Theses.Count + Journals.Count;
    }
}