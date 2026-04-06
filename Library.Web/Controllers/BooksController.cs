using System;
using System.Linq;
using System.Threading.Tasks;
using Library.Domain.Interfaces;
using Library.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class BooksController : Controller
{
    private readonly IStudentRepository _studentRepo;
    private readonly IBookRepository _bookRepo;

    public BooksController(IStudentRepository studentRepo, IBookRepository bookRepo)
    {
        _studentRepo = studentRepo;
        _bookRepo = bookRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var booksFromDb = await _studentRepo.GetAllBooksAsync();

        var viewModel = booksFromDb.Select(book => new BookViewModel
        {
            Id = book.BookId,
            Title = book.Title,
            Author = book.Author,
            Department = book.Department,
            Edition = book.Edition ?? "N/A",
            IsAvailable = book.AvailableCopies.HasValue && book.AvailableCopies.Value > 0,

            // 🔥 Added the new mappings for your detailed description area
            PublicationYear = book.PublicationYear ?? "N/A",
            TotalCopies = book.TotalCopies ?? 0,
            AvailableCopies = book.AvailableCopies ?? 0
        }).ToList();

        return View("~/Views/Books/Index.cshtml", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Borrow(int id)
    {
        int? currentStudentId = HttpContext.Session.GetInt32("CurrentStudentId");
        if (currentStudentId == null) return RedirectToAction("Login", "Account");

        bool success = await _studentRepo.BorrowBookAsync(currentStudentId.Value, id);

        if (!success) TempData["Error"] = "Unable to borrow book. It may be out of stock.";
        else TempData["Success"] = "Book borrowed successfully!";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Reserve(int id)
    {
        int? currentStudentId = HttpContext.Session.GetInt32("CurrentStudentId");
        if (currentStudentId == null) return RedirectToAction("Login", "Account");

        bool success = await _studentRepo.ReserveBookAsync(currentStudentId.Value, id);

        if (!success) TempData["Error"] = "Unable to process reservation.";
        else TempData["Success"] = "Book reserved successfully!";

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> MyBooks()
    {
        int? studentId = HttpContext.Session.GetInt32("CurrentStudentId");
        if (studentId == null) return RedirectToAction("Login", "Account");

        var allRecords = await _bookRepo.GetFullBorrowingHistoryAsync(studentId.Value);

        var viewModel = new MyBooksViewModel
        {
            // Filter for Active (Not Returned)
            ActiveBorrowedBooks = allRecords.Where(r => !r.IsReturned).Select(r => new ActiveBorrowedBookViewModel
            {
                BorrowingRecordId = r.RecordId,
                Title = r.Item?.Title ?? "Unknown Book",
                Author = r.Item?.Author ?? "Unknown",
                BorrowDate = r.BorrowDate,
                ExpectedReturnDate = r.ExpectedReturnDate
            }).ToList(),

            // Filter for History (Returned)
            PastReads = allRecords.Where(r => r.IsReturned).Select(r => new PastReadViewModel
            {
                Title = r.Item?.Title ?? "Unknown Book",
                Author = r.Item?.Author ?? "Unknown",
                BorrowDate = r.BorrowDate,
                ActualReturnDate = r.ActualReturnDate
            }).ToList()
        };

        return View("~/Views/Books/MyBooks.cshtml" , viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Return(int id)
    {
        await _bookRepo.ReturnBookAsync(id);
        return RedirectToAction("MyBooks");
    }

    [HttpPost]
    public async Task<IActionResult> Renew(int id)
    {
        await _bookRepo.RenewBookAsync(id, 7); // Extend by 1 week
        return RedirectToAction("MyBooks");
    }
}