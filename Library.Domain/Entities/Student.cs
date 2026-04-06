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

    // NEW AUTHENTICATION FIELDS
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = "Student";

    // Navigation Property
    public virtual ICollection<Borrowingrecord> BorrowingRecords { get; set; } = new List<Borrowingrecord>();
}