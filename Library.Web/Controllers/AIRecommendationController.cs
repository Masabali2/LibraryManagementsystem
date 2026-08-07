using Library.Domain.Interfaces;
using Library.Infrastructure.Services;
using Library.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class AIRecommendationController : Controller
{
    private readonly RecommendationApiService _service;
    private readonly IBookRepository _bookRepository;

    public AIRecommendationController(
        RecommendationApiService service,
        IBookRepository bookRepository)
    {
        _service = service;
        _bookRepository = bookRepository;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = "AISuggestions";

        // Get currently logged-in student from session
        var studentId = HttpContext.Session.GetInt32("CurrentStudentId");

        if (studentId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Call FastAPI
        var recommendations = await _service.GetRecommendationsAsync(studentId.Value);

        if (recommendations == null || !recommendations.Any())
        {
            return View(new List<RecommendedBookViewModel>());
        }

        var bookIds = recommendations.Select(x => x.BookId).ToList();

        var books = await _bookRepository.GetBooksByIdsAsync(bookIds);

        var model = recommendations
            .Select(r =>
            {
                var book = books.FirstOrDefault(b => b.BookId == r.BookId);

                if (book == null)
                    return null;

                return new RecommendedBookViewModel
                {
                    Book = book,
                    Score = r.Score,
                    Reason = r.Reason
                };
            })
            .Where(x => x != null)
            .Cast<RecommendedBookViewModel>()
            .ToList();

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var studentId = HttpContext.Session.GetInt32("CurrentStudentId");
        if (studentId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var books = await _bookRepository.GetBooksByIdsAsync(new List<int> { id });
        var book = books.FirstOrDefault();

        if (book == null)
        {
            return NotFound();
        }

        // Check active borrowing or request history using your repository method
        var history = await _bookRepository.GetFullBorrowingHistoryAsync(studentId.Value);
        var activeRecord = history.FirstOrDefault(r => r.ItemId == id && !r.IsReturned && r.Status != "Rejected");

        var viewModel = new BookDetailsViewModel
        {
            Book = book,
            UserActiveStatus = activeRecord?.Status,
            IsAlreadyBorrowedOrReserved = activeRecord != null
        };

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_BookDetailsModal", viewModel);
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BorrowOrReserve(int bookId, string actionType)
    {
        try
        {
            var studentId = HttpContext.Session.GetInt32("CurrentStudentId");

            if (studentId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Please login first."
                });
            }

            var success = await _bookRepository.CreateRequestAsync(
                studentId.Value,
                bookId,
                "Book",
                actionType);

            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = $"{actionType} request submitted successfully."
                });
            }

            return Json(new
            {
                success = false,
                message = "Unable to submit request."
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}
