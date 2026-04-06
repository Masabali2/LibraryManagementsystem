using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int StudentId { get; set; }

       
        public int ItemId { get; set; }
        public string ItemType { get; set; } = null!;

        public DateTime ReservationDate { get; set; }
        public string Status { get; set; } = "Active"; // "Active", "Completed", "Cancelled"

        // Relationship
        public virtual Student Student { get; set; } = null!;
    }
}
