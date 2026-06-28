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
        // Fetch them one by one to avoid Concurrency errors
        var books = await _studentRepo.GetAllBooksAsync();
        var journals = await _studentRepo.GetAllJournalsAsync();
        var theses = await _studentRepo.GetAllThesesAsync();

        // Map Books
        var bookVMs = books.Select(b => new BookViewModel
        {
            Id = b.BookId,
            Title = b.Title,
            Author = b.Author,
            Department = b.Department,
            Edition = b.Edition ?? "N/A",
            PublicationYear = b.PublicationYear ?? "N/A",
            AvailableCopies = b.AvailableCopies ?? 0,
            TotalCopies = b.TotalCopies ?? 0,
            IsAvailable = (b.AvailableCopies ?? 0) > 0,
            ItemType = "Book",
            ImageUrl= b.ImageUrl // Map the image URL if available
        });

        // Map Journals
        var journalVMs = journals.Select(j => new BookViewModel
        {
            Id = j.JournalId,
            Title = j.JournalName,
            Author = j.Publisher ?? "N/A",
            Department = j.Department ?? "General",
            Edition = $"Vol. {j.Volume}",
            PublicationYear = j.Year,
            AvailableCopies = j.Quantity ?? 0,
            TotalCopies = j.Quantity ?? 0,
            IsAvailable = (j.Quantity ?? 0) > 0,
            ItemType = "Journal",
            ImageUrl = j.ImageUrl // Map the image URL if available
        });

        // Map Theses
        var thesisVMs = theses.Select(t => new BookViewModel
        {
            Id = t.ThesisId,
            Title = t.Title,
            Author = t.StudentName,
            Department = t.Department,
            Edition = t.Batch,
            PublicationYear = t.Year.ToString(),
            AvailableCopies = 1,
            TotalCopies = 1,
            IsAvailable = true,
            ItemType = "Thesis",
            ImageUrl = t.ImageUrl // Map the image URL if available
        });

        var finalModel = bookVMs.Concat(journalVMs).Concat(thesisVMs)
                                .OrderBy(x => x.Title)
                                .ToList();

        return View(finalModel);
    }

    /// <summary>
    /// Unified method to handle Borrow and Reserve requests for all item types
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ProcessRequest(int id, string itemType, string requestType)
    {
        int? currentStudentId = HttpContext.Session.GetInt32("CurrentStudentId");
        if (currentStudentId == null) return RedirectToAction("Login", "Account");

        bool success = false;

        // We pass itemType (Book, Journal, Thesis) so the Repository knows which table to hit
        if (requestType == "Borrow")
        {
            success = await _studentRepo.BorrowItemAsync(currentStudentId.Value, id, itemType);
        }
        else if (requestType == "Reserve")
        {
            success = await _studentRepo.ReserveItemAsync(currentStudentId.Value, id, itemType);
        }

        if (!success)
        {
            TempData["Error"] = $"Unable to {requestType.ToLower()} this {itemType.ToLower()}.";
        }
        else
        {
            // Success message for "Wait for Admin" requirement
            TempData["Success"] = $"Your {requestType} request for the {itemType} has been submitted. Please wait for Admin approval.";
        }

        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> MyBooks()
    {
        int? studentId = HttpContext.Session.GetInt32("CurrentStudentId");
        if (studentId == null) return RedirectToAction("Login", "Account");

        // 1. Get the base records from BorrowingHistory
        var allRecords = await _bookRepo.GetFullBorrowingHistoryAsync(studentId.Value);

        // 2. Fetch all catalogs to map the names (simpler than complex joins)
        var books = await _studentRepo.GetAllBooksAsync();
        var journals = await _studentRepo.GetAllJournalsAsync();
        var theses = await _studentRepo.GetAllThesesAsync();

        // 3. Map the data with specific titles and types
        var mappedItems = allRecords.Select(r => {
            string title = "Unknown";
            string author = "Unknown";

            if (r.ItemType == "Book")
            {
                var b = books.FirstOrDefault(x => x.BookId == r.ItemId);
                title = b?.Title ?? "Unknown Book";
                author = b?.Author ?? "N/A";
            }
            else if (r.ItemType == "Journal")
            {
                var j = journals.FirstOrDefault(x => x.JournalId == r.ItemId);
                title = j?.JournalName ?? "Unknown Journal";
                author = j?.Publisher ?? "N/A";
            }
            else if (r.ItemType == "Thesis")
            {
                var t = theses.FirstOrDefault(x => x.ThesisId == r.ItemId);
                title = t?.Title ?? "Unknown Thesis";
                author = t?.StudentName ?? "N/A";
            }

            return new { Record = r, Title = title, Author = author };
        }).ToList();

        var viewModel = new MyBooksViewModel
        {
            // ... inside your MyBooks method, update the mapping:
            ActiveBorrowedBooks = mappedItems.Where(x => !x.Record.IsReturned).Select(x => new ActiveBorrowedBookViewModel
            {
                BorrowingRecordId = x.Record.RecordId,
                Title = x.Title,
                Author = x.Author,
                Type = x.Record.ItemType,
                // Add this line to grab the image based on type
                ImageUrl = x.Record.ItemType == "Book" ? books.FirstOrDefault(b => b.BookId == x.Record.ItemId)?.ImageUrl :
                           x.Record.ItemType == "Journal" ? journals.FirstOrDefault(j => j.JournalId == x.Record.ItemId)?.ImageUrl :
                           theses.FirstOrDefault(t => t.ThesisId == x.Record.ItemId)?.ImageUrl,
                BorrowDate = x.Record.BorrowDate,
                ExpectedReturnDate = x.Record.ExpectedReturnDate
            }).ToList(),

            PastReads = mappedItems.Where(x => x.Record.IsReturned).Select(x => new PastReadViewModel
            {
                Title = x.Title,
                Author = x.Author,
                Type = x.Record.ItemType,
                BorrowDate = x.Record.BorrowDate,
                ActualReturnDate = x.Record.ActualReturnDate,
                // Add the mapping logic here
                ImageUrl = x.Record.ItemType == "Book" ? books.FirstOrDefault(b => b.BookId == x.Record.ItemId)?.ImageUrl :
                x.Record.ItemType == "Journal" ? journals.FirstOrDefault(j => j.JournalId == x.Record.ItemId)?.ImageUrl :
                theses.FirstOrDefault(t => t.ThesisId == x.Record.ItemId)?.ImageUrl
            }).ToList()
        };

        return View("~/Views/Books/MyBooks.cshtml", viewModel);
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
        await _bookRepo.RenewBookAsync(id, 7);
        return RedirectToAction("MyBooks");
    }
}