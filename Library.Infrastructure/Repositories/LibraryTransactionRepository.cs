using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class LibraryTransactionRepository : ILibraryTransactionRepository
{
    private readonly LibraryDbContext _context;

    public LibraryTransactionRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateTransactionAsync(LibraryTransaction transaction)
    {
        try
        {
            await _context.LibraryTransactions.AddAsync(transaction);
            return await _context.SaveChangesAsync() > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<LibraryTransaction?> GetTransactionByIdAsync(int transactionId)
    {
        return await _context.LibraryTransactions
            .Include(t => t.Student)
            .FirstOrDefaultAsync(t => t.LibraryTransactionId == transactionId);
    }

    public async Task<List<LibraryTransaction>> GetTransactionsByStudentIdAsync(int studentId)
    {
        return await _context.LibraryTransactions
            .Include(t => t.Student)
            .Where(t => t.StudentId == studentId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<LibraryTransaction>> GetAllTransactionsAsync()
    {
        return await _context.LibraryTransactions
            .Include(t => t.Student)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateTransactionStatusAsync(int transactionId, string status)
    {
        var transaction = await _context.LibraryTransactions
            .FirstOrDefaultAsync(t => t.LibraryTransactionId == transactionId);

        if (transaction == null)
            return false;

        transaction.Status = status;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteTransactionAsync(int transactionId)
    {
        var transaction = await _context.LibraryTransactions
            .FirstOrDefaultAsync(t => t.LibraryTransactionId == transactionId);

        if (transaction == null)
            return false;

        _context.LibraryTransactions.Remove(transaction);

        return await _context.SaveChangesAsync() > 0;
    }
}