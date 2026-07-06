using Library.Domain.Entities;

namespace Library.Domain.DTOs;

public class AdminStudentDetailsDto
{
    public Student Student { get; set; } = null!;

    public int BorrowedBooksCount { get; set; }
    public int ReservedBooksCount { get; set; }
    public decimal PendingFines { get; set; }
    public decimal ManualFineAmount { get; set; }
    public IEnumerable<Borrowingrecord> BorrowedRecords { get; set; } = new List<Borrowingrecord>();
    public IEnumerable<Reservation> Reservations { get; set; } = new List<Reservation>();
}