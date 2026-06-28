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
            .Where(r => r.StudentId == studentId && (r.Status == "Active" || r.Status == "Pending"))
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
    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _context.Books.ToListAsync();
    }
    public async Task<IEnumerable<Journal>> GetAllJournalsAsync()
    {
        return await _context.Journals.ToListAsync();
    }

    public async Task<IEnumerable<Thesis>> GetAllThesesAsync()
    {
        return await _context.Theses.ToListAsync();
    }
    public async Task<IEnumerable<Borrowingrecord>> GetBorrowedItemsByStudentIdAsync(int studentId)
    {
        var records = await _context.BorrowingRecords
            .Where(b => b.StudentId == studentId && b.IsReturned == false)
            .ToListAsync();

        foreach (var record in records)
        {
            if (record.ItemType == "Book")
            {
                var book = await _context.Books.FindAsync(record.ItemId);
                record.Title = book?.Title ?? "Unknown Book";
                record.ImageUrl = book?.ImageUrl; // Add this
            }
            else if (record.ItemType == "Journal")
            {
                var journal = await _context.Journals.FindAsync(record.ItemId);
                record.Title = journal?.JournalName ?? "Unknown Journal";
                record.ImageUrl = journal?.ImageUrl; // Add this
            }
            else if (record.ItemType == "Thesis")
            {
                var thesis = await _context.Theses.FindAsync(record.ItemId);
                record.Title = thesis?.Title ?? "Unknown Thesis";
                record.ImageUrl = thesis?.ImageUrl; // Add this
            }
        }
        return records;
    }
    public async Task<bool> BorrowItemAsync(int studentId, int itemId, string itemType)
    {
        // 1. Check if the specific item exists and is available based on type
        if (itemType == "Book")
        {
            var book = await _context.Books.FindAsync(itemId);
            if (book == null || book.AvailableCopies <= 0) return false;
            book.AvailableCopies--;
        }
        else if (itemType == "Journal")
        {
            var journal = await _context.Journals.FindAsync(itemId);
            if (journal == null || journal.Quantity <= 0) return false;
            journal.Quantity--;
        }
        else if (itemType == "Thesis")
        {
            // Theses are usually unique (1 copy), so just check if it's already borrowed
            var alreadyBorrowed = await _context.BorrowingRecords
                .AnyAsync(r => r.ItemId == itemId && r.ItemType == "Thesis" && !r.IsReturned);
            if (alreadyBorrowed) return false;
        }

        // 2. Create the Borrowing Record
        var record = new Borrowingrecord
        {
            StudentId = studentId,
            ItemId = itemId,
            ItemType = itemType, // Crucial: Store the type so the Dashboard knows what it is!
            BorrowDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Status = "Pending", // For your "Wait for Admin" requirement
            IsReturned = false
        };

        _context.BorrowingRecords.Add(record);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> ReserveItemAsync(int studentId, int itemId, string itemType)
    {
        var reservation = new Reservation
        {
            StudentId = studentId,
            ItemId = itemId,
            ItemType = itemType,
            ReservationDate = DateTime.Now,
            ExpiryDate = DateTime.Now.AddDays(7),
            Status = "Pending" // Triggers "Wait for Admin" on UI
        };

        await _context.Reservations.AddAsync(reservation);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<IEnumerable<Reservation>> GetActiveReservationsByStudentIdAsync(int studentId)
    {
        var reservations = await _context.Reservations
            .Where(r => r.StudentId == studentId)
            .ToListAsync();

        foreach (var res in reservations)
        {
            if (res.ItemType == "Book")
            {
                var item = await _context.Books.FindAsync(res.ItemId);
                res.Title = item?.Title ?? "Unknown Book";
                res.ImageUrl = item?.ImageUrl; // Add this
            }
            else if (res.ItemType == "Journal")
            {
                var item = await _context.Journals.FindAsync(res.ItemId);
                res.Title = item?.JournalName ?? "Unknown Journal";
                res.ImageUrl = item?.ImageUrl; // Add this
            }
            else if (res.ItemType == "Thesis")
            {
                var item = await _context.Theses.FindAsync(res.ItemId);
                res.Title = item?.Title ?? "Unknown Thesis";
                res.ImageUrl = item?.ImageUrl; // Add this
            }
        }
        return reservations;
    }

    public async Task<Student> GetStudentByIdAsync(int studentId)
    {
        return await _context.Students
               .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }
    public async Task<bool> UpdateStudentAsync(Student student)
    {
        // Find if the entity is already tracked in the context to prevent identity conflicts
        var trackedEntity = await _context.Students.FindAsync(student.StudentId);

        if (trackedEntity != null)
        {
            // Detach the existing instance from change tracking engine pipeline maps
            _context.Entry(trackedEntity).State = EntityState.Detached;
        }

        // Attach current updated model and mark explicit entity modified state updates
        _context.Entry(student).State = EntityState.Modified;

        int rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }
 
    public async Task<SeatAvailability?> GetSeatAvailabilityAsync()
    {
        // Fetches the live record updated by your Python AI
        // AsNoTracking is used because we don't need to save changes back from this specific call
        return await _context.SeatAvailabilities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == 1);
    }

}