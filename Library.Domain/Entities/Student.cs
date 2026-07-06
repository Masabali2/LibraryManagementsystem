using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    // 🚀 NEW PROPERTY: Stores the file path or URL of the cover image
    [StringLength(1000)]
    public string? ImageUrl { get; set; }
    public bool IsBanned { get; set; } = false;
    public string? BanReason { get; set; }
}