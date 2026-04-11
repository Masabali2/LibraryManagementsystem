using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Library.Domain.Entities;

namespace Library.Domain.Interfaces;

public interface IAdminRepository
{
    // We use Task for asynchronous database operations
    Task<Admin?> GetAdminByCredentialsAsync(string username, string password);
  
    Task<int> GetTotalBooksCountAsync();
    Task<int> GetActiveStudentsCountAsync();
    Task<int> GetBooksIssuedTodayCountAsync();
    Task<int> GetOverdueReturnsCountAsync();
    Task<decimal> GetTotalUnpaidFinesAsync();

    // List for the Table
    Task<List<Borrowingrecord>> GetRecentBorrowingRecordsAsync(int count);
}
