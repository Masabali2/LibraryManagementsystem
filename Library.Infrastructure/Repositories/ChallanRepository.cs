using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class ChallanRepository : IChallanRepository
{
    private readonly LibraryDbContext _context;

    public ChallanRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateChallanAsync(Challan challan)
    {
        if (challan == null)
            return false;

        await _context.Challans.AddAsync(challan);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Challan?> GetChallanByIdAsync(int challanId)
    {
        return await _context.Challans
            .Include(c => c.Student)
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.ChallanId == challanId);
    }

    public async Task<List<Challan>> GetChallansByStudentIdAsync(int studentId)
    {
        return await _context.Challans
            .Include(c => c.Items)
            .Where(c => c.StudentId == studentId)
            .OrderByDescending(c => c.IssueDate)
            .ToListAsync();
    }

    public async Task<List<Challan>> GetAllChallansAsync()
    {
        return await _context.Challans
            .Include(c => c.Student)
            .Include(c => c.Items)
            .OrderByDescending(c => c.IssueDate)
            .ToListAsync();
    }

    public async Task<bool> UpdateChallanStatusAsync(int challanId, string status)
    {
        var challan = await _context.Challans.FindAsync(challanId);

        if (challan == null)
            return false;

        challan.Status = status;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteChallanAsync(int challanId)
    {
        var challan = await _context.Challans
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.ChallanId == challanId);

        if (challan == null)
            return false;

        _context.ChallanItems.RemoveRange(challan.Items);
        _context.Challans.Remove(challan);

        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<int> GetUnpaidChallanCountByStudentIdAsync(int studentId)
    {
        return await _context.Challans
            .CountAsync(c => c.StudentId == studentId && c.Status != "Paid");
    }
}