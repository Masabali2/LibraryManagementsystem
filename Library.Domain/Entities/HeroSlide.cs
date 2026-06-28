using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Library.Domain.Entities;

public class HeroSlide
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(1000)]
    public string ImageUrl { get; set; } = null!; // Path to the banner image

    [StringLength(200)]
    public string? Title { get; set; } // Optional overlay title (e.g., "Welcome to Wollondilly Library")

    [StringLength(500)]
    public string? Description { get; set; } // Optional overlay description text

    public int DisplayOrder { get; set; } // Controls which image shows first (1, 2, 3...)

    public bool IsActive { get; set; } = true; // Easily hide/show slides
}