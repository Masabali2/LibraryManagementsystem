using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Library.Domain.Entities;

public class Journal
{
    public int JournalId { get; set; }
    [StringLength(1000)]
    public string JournalName { get; set; } = null!;
    public string? Department { get; set; }
    public string? Publisher { get; set; }
    public int? Volume { get; set; }
    public string? Edition { get; set; }
    public string Year { get; set; }
    public int? Quantity { get; set; }
    public int? ShelfId { get; set; } // Nullable so old data doesn't crash
    public virtual Shelf? Shelf { get; set; }
    // 🚀 NEW PROPERTY: Stores the file path or URL of the cover image
    [StringLength(1000)]
    public string? ImageUrl { get; set; }
}