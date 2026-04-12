using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Domain.Entities;

namespace Library.Domain.Interfaces;

public interface IAdminRepository
{
    // Authentication
    Task<Admin?> GetAdminByCredentialsAsync(string username, string password);

    // Main Stats Cards
    Task<int> GetTotalBooksCountAsync();
    Task<int> GetActiveStudentsCountAsync();
    Task<int> GetBooksIssuedTodayCountAsync();
    Task<int> GetOverdueReturnsCountAsync();
    Task<decimal> GetTotalUnpaidFinesAsync();

    // New: Inventory Breakdown Stats
    // These fix the missing property issues in your ViewModel
    Task<int> GetSpecificBooksCountAsync();     // Regular books
    Task<int> GetThesisCountAsync();            // Thesis items
    Task<int> GetJournalCountAsync();           // Journal items

    // List for the Table
    // Updated to return records including related Student and Asset data
    Task<List<Borrowingrecord>> GetRecentBorrowingRecordsAsync(int count);
}