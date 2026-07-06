using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class StudentController : Controller
{
    private readonly IStudentRepository _studentRepo;
    private readonly IChallanRepository _challanRepository;

    // The repository is injected right here!
    public StudentController(
     IStudentRepository studentRepo,
     IChallanRepository challanRepository)
    {
        _studentRepo = studentRepo;
        _challanRepository = challanRepository;
    }
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        int? currentStudentId = HttpContext.Session.GetInt32("CurrentStudentId");
        if (currentStudentId == null) return RedirectToAction("Login", "Account");
        var student = await _studentRepo.GetStudentByIdAsync(currentStudentId.Value);

        // 1. Fetch from BOTH tables
        var borrowedRecords = await _studentRepo.GetBorrowedItemsByStudentIdAsync(currentStudentId.Value);
        var reservationRecords = await _studentRepo.GetActiveReservationsByStudentIdAsync(currentStudentId.Value);
        var studentChallans = await _challanRepository.GetChallansByStudentIdAsync(currentStudentId.Value);

        var pendingFines = studentChallans
            .Where(c => c.Status == "Unpaid" || c.Status == "Partially Paid")
            .Sum(c => c.TotalAmount);

        var viewModel = new StudentDashboardViewModel
        {
            StudentName = HttpContext.Session.GetString("StudentName") ?? "Student",
            ImageUrl = student?.ImageUrl,

            // 2. Fix Counts: Combine both lists to see total interaction per category
            BooksCount = borrowedRecords.Count(i => i.ItemType == "Book") + reservationRecords.Count(i => i.ItemType == "Book"),
            ThesesCount = borrowedRecords.Count(i => i.ItemType == "Thesis") + reservationRecords.Count(i => i.ItemType == "Thesis"),
            JournalsCount = borrowedRecords.Count(i => i.ItemType == "Journal") + reservationRecords.Count(i => i.ItemType == "Journal"),

            // 3. Fix Reservation Count
            ActiveReservationsCount = reservationRecords.Count(),
            PendingApprovalsCount = borrowedRecords.Count(i => i.Status == "Pending") + reservationRecords.Count(i => i.Status == "Pending"),

            PendingFines = pendingFines,

            // 4. Map the lists correctly
            BorrowedItems = borrowedRecords.Select(item => new BorrowedItemViewModel
            {
                Title = item.Title, // Use the Title filled by your Repository foreach loop
                Type = item.ItemType,
                ImageUrl = item.ImageUrl,
                BorrowedDate = item.BorrowDate,
                DueDate = item.ExpectedReturnDate ?? item.BorrowDate.AddDays(14),
                Status = item.Status
            }).ToList(),

            ReservedItems = reservationRecords
        };
        ViewData["ActivePage"] = "Dashboard";

        ViewBag.UnpaidChallanCount =
            await _challanRepository.GetUnpaidChallanCountByStudentIdAsync(currentStudentId.Value);

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(Student updatedStudent, IFormFile? profileImageFile)
    {
        if (ModelState.IsValid)
        {
            if (profileImageFile != null && profileImageFile.Length > 0)
            {
                // 1. Build path to target subdirectory: wwwroot/images/{studentId}
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", updatedStudent.StudentId.ToString());

                // Ensure the directory structure exists on disk
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 2. Extract extension (.jpg, .png, etc.) and build clean filename
                string extension = Path.GetExtension(profileImageFile.FileName);
                string fileName = $"profile_image{extension}";
                string fullPhysicalPath = Path.Combine(folderPath, fileName);

                // 3. Write binary data block payload safely to local server storage
                using (var stream = new FileStream(fullPhysicalPath, FileMode.Create))
                {
                    await profileImageFile.CopyToAsync(stream);
                }

                // 4. Update the path pointing to the stored asset image
                updatedStudent.ImageUrl = $"/images/{updatedStudent.StudentId}/{fileName}";
            }

            // Save the modifications right down into the underlying repository context
            bool success = await _studentRepo.UpdateStudentAsync(updatedStudent);

            if (success)
            {
                // Update the live session strings for layouts
                HttpContext.Session.SetString("StudentName", updatedStudent.StudentName);
                HttpContext.Session.SetString("StudentImageUrl", updatedStudent.ImageUrl ?? "");

                TempData["SuccessMessage"] = "Profile and image updated successfully!";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", "Unable to save changes. Please try again.");
        }

        // Fallback if validations or contexts fail
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