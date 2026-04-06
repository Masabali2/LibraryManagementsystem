using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class StudentController : Controller
{
    private readonly IStudentRepository _studentRepo;

    // The repository is injected right here!
    public StudentController(IStudentRepository studentRepo)
    {
        _studentRepo = studentRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        int? currentStudentId = HttpContext.Session.GetInt32("CurrentStudentId");

        if (currentStudentId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // 1. Fetching counts and student name
        var borrowedCount = await _studentRepo.GetBorrowedBooksCountAsync(currentStudentId.Value);
        var reservationCount = await _studentRepo.GetActiveReservationsCountAsync(currentStudentId.Value);
        var pendingFines = await _studentRepo.GetPendingFinesAsync(currentStudentId.Value);
        var studentName = HttpContext.Session.GetString("StudentName")
            ?? await _studentRepo.GetStudentNameByIdAsync(currentStudentId.Value);

        // 2. Fetching the raw borrowed items AND all books 
        var dbBorrowedItems = await _studentRepo.GetBorrowedItemsByStudentIdAsync(currentStudentId.Value);
        var allBooks = await _studentRepo.GetAllBooksAsync(); // 👈 This grabs all books to compare titles

        // 3. Mapping the DB data over to your pretty Frontend ViewModel
        var viewModel = new StudentDashboardViewModel
        {
            StudentName = studentName,
            BorrowedBooksCount = borrowedCount,
            ActiveReservationsCount = reservationCount,
            PendingFines = pendingFines,
            BorrowedItems = dbBorrowedItems.Select(item =>
            {
                // Find the book in your database where the IDs match
                var matchingBook = allBooks.FirstOrDefault(b => b.BookId == item.ItemId);

                return new BorrowedItemViewModel
                {
                    // Grabs the real title!
                    Title = matchingBook != null ? matchingBook.Title : $"{item.ItemType} #{item.ItemId}",
                    ItemType = item.ItemType,
                    BorrowedDate = item.BorrowDate,

                    //  FIXED: Dynamically calculates the 14-day due date from the day it was borrowed
                    DueDate = item.BorrowDate.AddDays(14),

                    //  FIXED: Calculates if the book is due in 3 days or less
                    IsDueSoon = item.BorrowDate.AddDays(14) <= DateTime.Now.AddDays(3)
                };
            }).ToList()
        };

        return View("~/Views/Student/Dashboard.cshtml", viewModel);
    }
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        // 1. Get the current student's ID from the session we set at login
        int? studentId = HttpContext.Session.GetInt32("CurrentStudentId");

        if (studentId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // 2. Fetch student details from the repository
        var student = await _studentRepo.GetStudentByIdAsync(studentId.Value);

        if (student == null)
        {
            return NotFound();
        }

        ViewData["ActivePage"] = "Profile";
        return View(student);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(Student updatedStudent)
    {
        if (ModelState.IsValid)
        {
            // Save the updated object directly to the database
            bool success = await _studentRepo.UpdateStudentAsync(updatedStudent);

            if (success)
            {
                // Keep the live top bar updated with the new name and username session
                HttpContext.Session.SetString("StudentName", updatedStudent.StudentName);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", "Unable to save changes. Please try again.");
        }

        // If something fails, return to the profile view with the errors
        return View("Profile", updatedStudent);
    }
}