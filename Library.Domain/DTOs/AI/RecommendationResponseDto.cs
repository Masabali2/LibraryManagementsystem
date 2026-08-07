using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.DTOs.AI
{

    public class RecommendationResponseDto
    {
        public int StudentId { get; set; }

        public int RecommendationCount { get; set; }

        public List<BookRecommendationDto> Recommendations { get; set; } = new();
    }
}
