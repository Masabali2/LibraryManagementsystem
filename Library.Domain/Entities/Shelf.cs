using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class Shelf
    {
        public int ShelfId { get; set; }

        [Required, StringLength(50)]
        public string ShelfCode { get; set; } = null!; // e.g., "S-101", "Row-A1"

        public int LocationBlockId { get; set; }
        public LocationBlock LocationBlock { get; set; } = null!;

        // Navigation properties back to your assets
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<Journal> Journals { get; set; } = new List<Journal>();
        public ICollection<Thesis> Theses { get; set; } = new List<Thesis>();
    }
}
