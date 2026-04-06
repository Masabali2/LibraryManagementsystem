using System.Collections.Generic;
using Library.Domain.Entities; 

namespace Library.Web.ViewModels;

public class StudentDashboardViewModel
{
    public string StudentName { get; set; } = string.Empty;
    public int BorrowedBooksCount { get; set; }
    public int ActiveReservationsCount { get; set; }
    public decimal PendingFines { get; set; }
    public List<BorrowedItemViewModel> BorrowedItems { get; set; } = new List<BorrowedItemViewModel>();
}