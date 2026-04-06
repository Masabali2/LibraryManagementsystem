using System;
using System.Collections.Generic;
namespace Library.Domain.Entities;

public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int? PublicationYear { get; set; }
    public int? Edition { get; set; }
    public int? TotalCopies { get; set; }
    public int? AvailableCopies { get; set; }
}
