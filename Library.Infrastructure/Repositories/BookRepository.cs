using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
        record.Status = "Returned"; // Update status for tracking

        // 🚀 Handle Inventory based on ItemType
        if (record.ItemType == "Book")
        {
            var book = await _context.Books.FindAsync(record.ItemId);
            if (book != null) book.AvailableCopies += 1;
        }
        else if (record.ItemType == "Journal")
        {
            var journal = await _context.Journals.FindAsync(record.ItemId);
            if (journal != null) journal.Quantity += 1;
        }
        // Theses usually only have 1 copy, so we just toggle availability if needed

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RenewBookAsync(int recordId, int daysToExtend)
    {
        var record = await _context.BorrowingRecords.FindAsync(recordId);

        if (record == null || record.IsReturned) return false;

        record.ExpectedReturnDate = record.ExpectedReturnDate?.AddDays(daysToExtend)
                                    ?? DateTime.Now.AddDays(daysToExtend);

        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<IEnumerable<Book>> GetFeaturedBooksAsync(int count)
    {
        return await _context.Books
            .Take(count)
            .ToListAsync();
    }
    public async Task<bool> CreateRequestAsync(int studentId, int itemId, string itemType, string requestType)
    {
        // Adjust entity and property names to match your database context structure
        var borrowingRecord = new Borrowingrecord
        {
            StudentId = studentId,
            ItemId = itemId,
            ItemType = itemType,// or ItemId depending on your schema
            RequestType = requestType, // e.g., "Borrow" or "Reserve"
            Status = "Pending",
            BorrowDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14)
        };

        _context.BorrowingRecords.Add(borrowingRecord);
        int affectedRows = await _context.SaveChangesAsync();

        return affectedRows > 0;
    }
    public async Task<List<LibraryItemOptionDto>> GetAvailableLibraryItemsAsync()
    {
        return await _context.Books
            .Where(book => book.AvailableCopies > 0)
            .OrderBy(book => book.Title)
            .Select(book => new LibraryItemOptionDto
            {
                ItemId = book.BookId,
                Title = book.Title,
                ItemType = "Book"
            })
            .ToListAsync();
    }
    public async Task<List<Book>> GetBooksByIdsAsync(List<int> ids)
    {
        if (ids == null || !ids.Any())
            return new List<Book>();

        return await _context.Books
            .Where(b => ids.Contains(b.BookId))
            .ToListAsync();
    }
}