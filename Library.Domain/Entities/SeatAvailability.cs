using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class SeatAvailability
    {
        [Key]
        public int Id { get; set; }
        public int TotalChairs { get; set; } = 50;
        public int PersonsOccupied { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [NotMapped]
        public int FreeChairs => Math.Max(0, TotalChairs - PersonsOccupied);
    }
}
