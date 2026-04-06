using System.Threading.Tasks;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data; 
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly LibraryDbContext _context; 

    public StudentRepository(LibraryDbContext context)
    {
        _context = context;
    }
    public async Task<Student?> GetStudentByUsernameAsync(string username)
    {
        return await _context.Students
            .Where(x => x.Username == username )
            .FirstOrDefaultAsync();
    }
    public async Task<Student?> GetStudentByRollNoAsync(string rollNo)
    {
        return await _context.Students
            .Where(s=> s.RollNo == rollNo)
            .FirstOrDefaultAsync();
    }
    public async Task<bool> AddStudentAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        int rowsAffected = await _context.SaveChangesAsync();

        return rowsAffected > 0;
    }
    public async Task<int> GetBorrowedBooksCountAsync(int studentId)
    {
        return await _context.BorrowingRecords
            .Where(b => b.StudentId == studentId && b.IsReturned == false)
            .CountAsync();
    }

    public async Task<int> GetActiveReservationsCountAsync(int studentId)
    {
        return await _context.Reservations
            .Where(r => r.StudentId == studentId && r.Status == "Active")
            .CountAsync();
    }
    public async Task<string> GetStudentNameByIdAsync(int studentId)
    {
        var student = await _context.Students
            .Where(s => s.StudentId == studentId)
            .FirstOrDefaultAsync();
        return student != null ? student.StudentName : "Student";
    }
    public async Task<decimal> GetPendingFinesAsync(int studentId)
    {
        return await _context.Fines
            .Where(f => f.StudentId == studentId && f.IsPaid == false)
            .SumAsync(f => f.Amount);
    }
    public async Task<IEnumerable<Borrowingrecord>> GetBorrowedItemsByStudentIdAsync(int studentId)
    {
        return await _context.BorrowingRecords
            .Where(b => b.StudentId == studentId && b.IsReturned == false)
            .ToListAsync();
    }
    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _context.Books.ToListAsync();
    }

    public async Task<bool> BorrowBookAsync(int studentId, int bookId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null || book.AvailableCopies <= 0) return false;

        // 1. Subtract an available copy
        book.AvailableCopies -= 1;

        // 2. Create a borrowing record
        var record = new Borrowingrecord
        {
            StudentId = studentId,
            ItemId = bookId,
            ItemType = "Book",
            BorrowDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14), // Default 2 week borrow
            IsReturned = false
        };

        await _context.BorrowingRecords.AddAsync(record);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ReserveBookAsync(int studentId, int bookId)
    {
        // Assuming you have a Reservations table mapped
        var reservation = new Reservation
        {
            StudentId = studentId,
            ItemId= bookId, 
            ReservationDate = DateTime.Now,
            Status = "Active",
            ItemType= "Book"
        };

        await _context.Reservations.AddAsync(reservation);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<Student> GetStudentByIdAsync(int studentId)
    {
        return await _context.Students
               .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }
    public async Task<bool> UpdateStudentAsync(Student student)
    {
        _context.Students.Update(student);
        int rowsAffected = await _context.SaveChangesAsync();

        return rowsAffected > 0;
    }

}