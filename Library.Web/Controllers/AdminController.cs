using Library.Domain.Interfaces;
using Library.Infrastructure.Repositories;
using Library.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class AdminController : Controller
{
    private readonly IAdminRepository _adminRepo;
    private readonly IChallanRepository _challanRepo;
    public AdminController(IAdminRepository adminRepo, IChallanRepository challanRepo)
    {
        _adminRepo = adminRepo;
        _challanRepo = challanRepo;
    }

    public async Task<IActionResult> Index()
    {
        var totalBooksCount = await _adminRepo.GetTotalBooksCountAsync();
        var activeStudents = await _adminRepo.GetActiveStudentsCountAsync();
        var issuedToday = await _adminRepo.GetBooksIssuedTodayCountAsync();
        var overdue = await _adminRepo.GetOverdueReturnsCountAsync();

        var specificBooksCount = await _adminRepo.GetSpecificBooksCountAsync();
        var thesesCount = await _adminRepo.GetThesisCountAsync();
        var journalsCount = await _adminRepo.GetJournalCountAsync();

        var recentBorrowing = await _adminRepo.GetRecentBorrowingRecordsAsync(50);
        var challans = await _challanRepo.GetAllChallansAsync();

        var totalRevenue = challans
            .Where(c => c.Status == "Paid")
            .Sum(c => c.TotalAmount);

        var viewModel = new AdminDashboardViewModel
        {
            TotalBooks = totalBooksCount,
            ActiveStudents = activeStudents,
            BooksIssuedToday = issuedToday,
            OverdueReturns = overdue,

            BooksCount = specificBooksCount,
            ThesesCount = thesesCount,
            JournalsCount = journalsCount,
            TotalRevenue=totalRevenue,

            RecentTransactions = recentBorrowing.Select(br =>
            {
                var detectedType = br.ItemType ?? "Book";

                var status = br.IsReturned
                    ? "Returned"
                    : br.Status == "Pending"
                        ? "Pending"
                        : "Issued";

                return new TransactionDto
                {
                    RecordId = br.RecordId,          // change to br.BorrowRecordId if your property name is different
                    StudentId = br.StudentId,

                    StudentName = br.Student?.StudentName ?? "Unknown Reader",

                    ItemId = br.ItemId.ToString(),
                    ItemName = br.Title ?? "System Asset",
                    ItemType = detectedType,

                    Date = br.BorrowDate,
                    Status = status
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