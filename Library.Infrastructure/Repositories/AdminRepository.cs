using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly LibraryDbContext _context;

    public AdminRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Admin?> GetAdminByCredentialsAsync(string username, string password)
    {
        return await _context.Admins
            .Where(u => u.Username == username && u.PasswordHash == password)
            .FirstOrDefaultAsync();
    }

    // --- Main Dashboard Stats ---

    public async Task<int> GetTotalBooksCountAsync() =>
        await _context.Books.CountAsync();

    public async Task<int> GetActiveStudentsCountAsync() =>
        await _context.Students.CountAsync();

    public async Task<int> GetBooksIssuedTodayCountAsync() =>
        await _context.BorrowingRecords.CountAsync(br => br.BorrowDate.Date == DateTime.Today);

    public async Task<int> GetOverdueReturnsCountAsync() =>
        await _context.BorrowingRecords.CountAsync(br => !br.IsReturned && br.ExpectedReturnDate < DateTime.Now);

    public async Task<decimal> GetTotalUnpaidFinesAsync() =>
        await _context.Fines.Where(f => !f.IsPaid).SumAsync(f => (decimal?)f.Amount) ?? 0m;

    // --- Inventory Breakdown (Student-Logic Style) ---

    public async Task<int> GetSpecificBooksCountAsync() =>
        await _context.Books.CountAsync();

    public async Task<int> GetThesisCountAsync() =>
        await _context.Theses.CountAsync();

    public async Task<int> GetJournalCountAsync() =>
        await _context.Journals.CountAsync();

    // --- Recent Activity Logic ---
    public async Task<List<Borrowingrecord>> GetRecentBorrowingRecordsAsync(int count)
    {
        var records = await _context.BorrowingRecords
            .Include(br => br.Student)
            .OrderByDescending(br => br.BorrowDate)
            .Take(count)
            .ToListAsync();

        foreach (var record in records)
        {
            // Manual lookup based on ItemType string
            if (record.ItemType == "Book")
            {
                record.Title = await _context.Books
                    .Where(b => b.BookId == record.ItemId)
                    .Select(b => b.Title)
                    .FirstOrDefaultAsync();
            }
            else if (record.ItemType == "Thesis")
            {
                record.Title = await _context.Theses
                    .Where(t => t.ThesisId == record.ItemId)
                    .Select(t => t.Title)
                    .FirstOrDefaultAsync();
            }
            else if (record.ItemType == "Journal")
            {
                record.Title = await _context.Journals
                    .Where(j => j.JournalId == record.ItemId)
                    .Select(j => j.JournalName)
                    .FirstOrDefaultAsync();
            }
        }

        return records;
    }
}