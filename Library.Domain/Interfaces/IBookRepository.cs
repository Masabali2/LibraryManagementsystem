using Library.Domain.DTOs;
using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Borrowingrecord>> GetFullBorrowingHistoryAsync(int studentId);
        Task<bool> ReturnBookAsync(int recordId);
        Task<List<Book>> GetBooksByIdsAsync(List<int> ids);
        Task<bool> RenewBookAsync(int recordId, int daysToExtend);
        Task<IEnumerable<Book>> GetFeaturedBooksAsync(int count);
        Task<bool> CreateRequestAsync(int studentId, int itemId, string itemType, string requestType);
        Task<List<LibraryItemOptionDto>> GetAvailableLibraryItemsAsync();
    }
}
