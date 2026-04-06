using Microsoft.EntityFrameworkCore;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;

namespace Library.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Borrowingrecord>> GetFullBorrowingHistoryAsync(int studentId)
    {
        return await _context.BorrowingRecords
            .Include(b => b.Item) // 🚀 THIS IS THE MAGIC LINE
            .Where(b => b.StudentId == studentId)
            .OrderByDescending(b => b.BorrowDate)
            .ToListAsync();
    }


    public async Task<bool> ReturnBookAsync(int recordId)
    {
        var record = await _context.BorrowingRecords.FindAsync(recordId);
        if (record == null) return false;

        record.IsReturned = true;
        record.ActualReturnDate = DateTime.Now;

        // 2. Put the book copy back into the available library pool
        var book = await _context.Books.FindAsync(record.ItemId);
        if (book != null)
        {
            book.AvailableCopies += 1;
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RenewBookAsync(int recordId, int daysToExtend)
    {
        var record = await _context.BorrowingRecords.FindAsync(recordId);

        // Safety checks: Record must exist and must not be returned already
        if (record == null || record.IsReturned) return false;

        // 1. Calculate and extend the ExpectedReturnDate
        record.ExpectedReturnDate = record.ExpectedReturnDate?.AddDays(daysToExtend)
                                    ?? DateTime.Now.AddDays(daysToExtend);

        return await _context.SaveChangesAsync() > 0;
    }
}