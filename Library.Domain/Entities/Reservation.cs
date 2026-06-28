using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DateTime ExpiryDate { get; set; }
        public DateTime ReservationDate { get; set; }
        [NotMapped]
        public string Title { get; set; }
      
            [NotMapped]
                public string ImageUrl { get; set; } // Temporary holder for the UI
       
        public string Status { get; set; } = "Active"; // "Active", "Completed", "Cancelled"

        // Relationship
        public virtual Student Student { get; set; } = null!;
    }
}
