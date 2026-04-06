using Library.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class Borrowingrecord
{
    public int RecordId { get; set; }
    public int StudentId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public bool IsReturned { get; set; }

    // Ensure this is used consistently: ItemType
    public string ItemType { get; set; } = null!;
    public int ItemId { get; set; }

    // Relationships
    public virtual Student Student { get; set; } = null!;

    // 🚀 ADD THIS: This links the record to the actual Book data
    [ForeignKey("ItemId")]
    public virtual Book? Item { get; set; }
}