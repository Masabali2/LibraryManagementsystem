using Library.Domain.Entities;

namespace Library.Domain.Interfaces;

public interface IChallanRepository
{
    // Create a challan with its charge items
    Task<bool> CreateChallanAsync(Challan challan);

    // Get one challan with Student and Items
    Task<Challan?> GetChallanByIdAsync(int challanId);

    // Get all challans for one student
    Task<List<Challan>> GetChallansByStudentIdAsync(int studentId);

    // Get all challans for admin/revenue tracking
    Task<List<Challan>> GetAllChallansAsync();

    // Update payment status: Unpaid / Partially Paid / Paid / Cancelled
    Task<bool> UpdateChallanStatusAsync(int challanId, string status);

    // Optional: delete an incorrect challan
    Task<bool> DeleteChallanAsync(int challanId);
    Task<int> GetUnpaidChallanCountByStudentIdAsync(int studentId);
}