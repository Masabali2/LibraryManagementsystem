using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Interfaces
{
    public interface IStudentRepository
    {
      
        Task<Student?> GetStudentByUsernameAsync(string username);

        Task<Student?> GetStudentByRollNoAsync(string rollNo);

        Task<string> GetStudentNameByIdAsync(int studentId);
        Task<bool> AddStudentAsync(Student student);
        Task<int> GetBorrowedBooksCountAsync(int studentId);
        Task<int> GetActiveReservationsCountAsync(int studentId);
        Task<decimal> GetPendingFinesAsync(int studentId);
        Task<IEnumerable<Borrowingrecord>> GetBorrowedItemsByStudentIdAsync(int studentId);
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<bool> BorrowBookAsync(int studentId, int bookId);
        Task<bool> ReserveBookAsync(int studentId, int bookId);
        Task<Student> GetStudentByIdAsync(int studentId);
        Task<bool> UpdateStudentAsync(Student student);
    }
}
