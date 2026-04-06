using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class Fine
    {

        public int FineId { get; set; }
        public int StudentId { get; set; }
        public int RecordId { get; set; } 
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; } = false;

        // Relationship
        public virtual Student Student { get; set; } = null!;
    }
}
