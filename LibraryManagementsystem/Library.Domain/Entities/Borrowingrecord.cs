using System;
using System.Collections.Generic;

namespace Library.Domain.Entities;

public class BorrowingRecord
{
    public int RecordId { get; set; }
    public int StudentId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public bool IsReturned { get; set; }
    public string ItemType { get; set; } = null!; // "Book", "Thesis", or "Journal"
    public int ItemId { get; set; }

    // Relationship
    public virtual Student Student { get; set; } = null!;
}