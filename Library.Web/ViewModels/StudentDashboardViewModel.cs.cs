using Library.Domain.Entities;

namespace Library.Web.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; } = string.Empty;

        // New Properties for the Multi-Asset Card
        public int BooksCount { get; set; }
        public int ThesesCount { get; set; }
        public int JournalsCount { get; set; }

        public int ActiveReservationsCount { get; set; }
        public decimal PendingFines { get; set; }
        public int PendingApprovalsCount { get; set; } 

        public List<BorrowedItemViewModel> BorrowedItems { get; set; } = new List<BorrowedItemViewModel>();
        public IEnumerable<Reservation> ReservedItems { get; set; } = new List<Reservation>();
    }
}