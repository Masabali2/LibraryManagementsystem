using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public async Task<int> GetTotalBooksCountAsync() =>
        await _context.Books.CountAsync();

    public async Task<int> GetActiveStudentsCountAsync() =>
        await _context.Students.CountAsync();

    public async Task<int> GetBooksIssuedTodayCountAsync() =>
        await _context.BorrowingRecords.CountAsync(br => br.BorrowDate.Date == DateTime.Today);

    public async Task<int> GetOverdueReturnsCountAsync() =>
        await _context.BorrowingRecords.CountAsync(br => !br.IsReturned && br.ExpectedReturnDate < DateTime.Now);

    public async Task<decimal> GetTotalUnpaidFinesAsync() =>
        await _context.Fines.Where(f => !f.IsPaid).SumAsync(f => f.Amount);

    public async Task<List<Borrowingrecord>> GetRecentBorrowingRecordsAsync(int count)
    {
        return await _context.BorrowingRecords
            .Include(br => br.Student)
            .Include(br => br.Item) // This uses the [ForeignKey("ItemId")] link you added
            .OrderByDescending(br => br.BorrowDate)
            .Take(count)
            .ToListAsync();
    }
}
