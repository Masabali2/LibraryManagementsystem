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
        var newRequest = new Borrowingrecord
        {
            StudentId = studentId,
            ItemId = itemId,
            ItemType = itemType,
            RequestType = requestType, 
            BorrowDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            IsReturned = false,
            Status = "Pending"
        };
        await _context.BorrowingRecords.AddAsync(newRequest);
        return await _context.SaveChangesAsync() > 0;
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
}