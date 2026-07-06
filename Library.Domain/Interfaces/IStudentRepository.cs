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
        Task<bool> AddStudentAsync(Student student);
        Task<int> GetBorrowedBooksCountAsync(int studentId);
        Task<int> GetActiveReservationsCountAsync(int studentId);
        Task<decimal> GetPendingFinesAsync(int studentId);
        Task<string> GetStudentNameByIdAsync(int studentId);
        Task<IEnumerable<Borrowingrecord>> GetBorrowedItemsByStudentIdAsync(int studentId);
        Task<IEnumerable<Reservation>> GetActiveReservationsByStudentIdAsync(int studentId);
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<IEnumerable<Journal>> GetAllJournalsAsync(); 
        Task<IEnumerable<Thesis>> GetAllThesesAsync();
        Task<Student> GetStudentByIdAsync(int studentId);
        Task<bool> BorrowItemAsync(int studentId, int itemId, string itemType);
        Task<bool> ReserveItemAsync(int studentId, int itemId, string itemType);
        Task<bool> UpdateStudentAsync(Student student);
        Task<SeatAvailability> GetSeatAvailabilityAsync();
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentDetailsByIdAsync(int studentId);
        Task<bool> AcceptBorrowRequestAsync(int recordId);
        Task<bool> ToggleStudentBanAsync(int studentId, string? reason);
    }
}
