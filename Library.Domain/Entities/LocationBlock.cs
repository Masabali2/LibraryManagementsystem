using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class LocationBlock
    {
        public int LocationBlockId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!; // e.g., "CS Block", "Main Hall"

        public string? Description { get; set; }

        // One Block can have many Shelves
        public ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
    }
}
