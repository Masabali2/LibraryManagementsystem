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
        if (currentStudentId == null) return RedirectToAction("Login", "Account");

        // 1. Fetch from BOTH tables
        var borrowedRecords = await _studentRepo.GetBorrowedItemsByStudentIdAsync(currentStudentId.Value);
        var reservationRecords = await _studentRepo.GetActiveReservationsByStudentIdAsync(currentStudentId.Value);

        var viewModel = new StudentDashboardViewModel
        {
            StudentName = HttpContext.Session.GetString("StudentName") ?? "Student",

            // 2. Fix Counts: Combine both lists to see total interaction per category
            BooksCount = borrowedRecords.Count(i => i.ItemType == "Book") + reservationRecords.Count(i => i.ItemType == "Book"),
            ThesesCount = borrowedRecords.Count(i => i.ItemType == "Thesis") + reservationRecords.Count(i => i.ItemType == "Thesis"),
            JournalsCount = borrowedRecords.Count(i => i.ItemType == "Journal") + reservationRecords.Count(i => i.ItemType == "Journal"),

            // 3. Fix Reservation Count
            ActiveReservationsCount = reservationRecords.Count(),
            PendingApprovalsCount = borrowedRecords.Count(i => i.Status == "Pending") + reservationRecords.Count(i => i.Status == "Pending"),

            PendingFines = await _studentRepo.GetPendingFinesAsync(currentStudentId.Value),

            // 4. Map the lists correctly
            BorrowedItems = borrowedRecords.Select(item => new BorrowedItemViewModel
            {
                Title = item.Title, // Use the Title filled by your Repository foreach loop
                Type = item.ItemType,
                BorrowedDate = item.BorrowDate,
                DueDate = item.ExpectedReturnDate ?? item.BorrowDate.AddDays(14),
                Status = item.Status
            }).ToList(),

            ReservedItems = reservationRecords
        };

        return View(viewModel);
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
    [HttpGet]
    public async Task<IActionResult> Seats()
    {
        ViewData["ActivePage"] = "Seats";
        var availability = await _studentRepo.GetSeatAvailabilityAsync();

        if (availability == null)
        {
            // Don't try to set FreeChairs here; 
            // it will calculate itself as (50 - 0) automatically
            return View(new SeatAvailability
            {
                TotalChairs = 50,
                PersonsOccupied = 0
            });
        }

        return View(availability);
    }

    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // Prevents stale data
    public async Task<JsonResult> GetLiveSeatData()
    {
        var data = await _studentRepo.GetSeatAvailabilityAsync();

        // We return the full set of data required by the new "Dashing" UI
        return Json(new
        {
            occupied = data?.PersonsOccupied ?? 0,
            total = data?.TotalChairs ?? 50,
            free = data?.FreeChairs ?? (data?.TotalChairs - data?.PersonsOccupied) ?? 50,
            lastSync = DateTime.Now.ToString("h:mm:ss tt") // Send server time for the "Last Update" label
        });
    }
}