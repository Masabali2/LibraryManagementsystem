
using System;
using System.Collections.Generic;
namespace Library.Domain.Entities;

public class Student
{
    public int StudentId { get; set; }
    public string RollNo { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Batch { get; set; } = null!;
    public string Department { get; set; } = null!;

    // Navigation Property
    public virtual ICollection<BorrowingRecord> BorrowingRecords { get; set; } = new List<BorrowingRecord>();
}
