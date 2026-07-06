using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Interfaces
{

    public interface ILibraryTransactionRepository
    {
        Task<bool> CreateTransactionAsync(LibraryTransaction transaction);

        Task<LibraryTransaction?> GetTransactionByIdAsync(int transactionId);

        Task<List<LibraryTransaction>> GetTransactionsByStudentIdAsync(int studentId);

        Task<List<LibraryTransaction>> GetAllTransactionsAsync();

        Task<bool> UpdateTransactionStatusAsync(int transactionId, string status);

        Task<bool> DeleteTransactionAsync(int transactionId);
    }
}
