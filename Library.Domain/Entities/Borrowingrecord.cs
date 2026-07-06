using Library.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Domain.Entities;

public class Borrowingrecord
{
    [Key]
    public int RecordId { get; set; }

    [Required]
    public int StudentId { get; set; }

    public DateTime BorrowDate { get; set; } = DateTime.Now;

    public DateTime? ExpectedReturnDate { get; set; }

    public DateTime? ActualReturnDate { get; set; }

    public bool IsReturned { get; set; } = false;

    [Required]
    public string ItemType { get; set; } = null!;

    [Required]
    public int ItemId { get; set; }
    [NotMapped]
    public string? Title { get; set; }
    [NotMapped]
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = "Pending";

    public string RequestType { get; set; } = "Borrow";

    [ForeignKey("StudentId")]
    public virtual Student Student { get; set; } = null!;

    [ForeignKey("ItemId")]
    public virtual Book? Item { get; set; }
}