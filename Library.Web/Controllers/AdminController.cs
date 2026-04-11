using Library.Domain.Interfaces;
using Library.Web.ViewModels; // Make sure this matches your folder structure
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class AdminController : Controller
{
    private readonly IAdminRepository _adminRepo;

    public AdminController(IAdminRepository adminRepo)
    {
        _adminRepo = adminRepo;
    }

    public async Task<IActionResult> Index()
    {
        // 1. Fetch data from Repository
        var totalBooks = await _adminRepo.GetTotalBooksCountAsync();
        var activeStudents = await _adminRepo.GetActiveStudentsCountAsync();
        var issuedToday = await _adminRepo.GetBooksIssuedTodayCountAsync();
        var overdue = await _adminRepo.GetOverdueReturnsCountAsync();
        var recentBorrowing = await _adminRepo.GetRecentBorrowingRecordsAsync(5);

        // 2. Map Domain Entities to ViewModel/DTO to avoid namespace errors in View
        var viewModel = new AdminDashboardViewModel
        {
            TotalBooks = totalBooks,
            ActiveStudents = activeStudents,
            BooksIssuedToday = issuedToday,
            OverdueReturns = overdue,
            RecentTransactions = recentBorrowing.Select(br => new TransactionDto
            {
                StudentName = br.Student?.StudentName ?? "Unknown User",
                BookId = $"BK-{br.ItemId}",
                Date = br.BorrowDate,
                Status = br.IsReturned ? "Returned" : "Issued"
            }).ToList()
        };

        return View(viewModel);
    }
}