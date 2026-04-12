using Library.Domain.Interfaces;
using Library.Web.ViewModels;
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
        // 1. Fetch main stats
        var totalBooksCount = await _adminRepo.GetTotalBooksCountAsync();
        var activeStudents = await _adminRepo.GetActiveStudentsCountAsync();
        var issuedToday = await _adminRepo.GetBooksIssuedTodayCountAsync();
        var overdue = await _adminRepo.GetOverdueReturnsCountAsync();

        // 2. Fetch inventory counts
        var specificBooksCount = await _adminRepo.GetSpecificBooksCountAsync();
        var thesesCount = await _adminRepo.GetThesisCountAsync();
        var journalsCount = await _adminRepo.GetJournalCountAsync();

        // 3. Fetch recent activity (Repository fills the [NotMapped] Title property)
        var recentBorrowing = await _adminRepo.GetRecentBorrowingRecordsAsync(10);

        // 4. Map to ViewModel
        var viewModel = new AdminDashboardViewModel
        {
            TotalBooks = totalBooksCount,
            ActiveStudents = activeStudents,
            BooksIssuedToday = issuedToday,
            OverdueReturns = overdue,

            BooksCount = specificBooksCount,
            ThesesCount = thesesCount,
            JournalsCount = journalsCount,

            RecentTransactions = recentBorrowing.Select(br => {

                // CORRECT LOGIC: Use the string property from your table
                // This ensures the JS filters "BOOK", "THESIS", "JOURNAL" work
                string detectedType = br.ItemType?.ToUpper() ?? "BOOK";

                return new TransactionDto
                {
                    StudentName = br.Student?.StudentName ?? "Unknown Reader",
                    ItemId = br.ItemId.ToString(),
                    // Use the Title property we filled manually in the Repository
                    ItemName = br.Title ?? "System Asset",
                    ItemType = detectedType,
                    Date = br.BorrowDate,
                    Status = br.IsReturned ? "Returned" : "Issued"
                };
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult BorrowThesis()
        => RedirectToAction("Create", "Borrowing", new { type = "Thesis" });

    [HttpGet]
    public IActionResult BorrowJournal()
        => RedirectToAction("Create", "Borrowing", new { type = "Journal" });
}