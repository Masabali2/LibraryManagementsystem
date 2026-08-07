using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.DTOs.AI
{
    public class BookRecommendationDto
    {
        public int BookId { get; set; }

        public string Title { get; set; } = "";

        public string Author { get; set; } = "";

        public string Department { get; set; } = "";

        public string? PublicationYear { get; set; }

        public int AvailableCopies { get; set; }

        public string? ImageUrl { get; set; }

        public double Score { get; set; }

        public int MatchPercentage { get; set; }

        public string Reason { get; set; } = "";
    }
}
