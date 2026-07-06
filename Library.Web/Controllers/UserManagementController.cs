using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Library.Application.Interfaces;
namespace Library.Web.Controllers;

public class UserManagementController : Controller
{
    private readonly IStudentRepository _studentRepository;
    private readonly IAuthService _authService;

    public UserManagementController(
     IStudentRepository studentRepository,
     IAuthService authService)
    {
        _studentRepository = studentRepository;
        _authService = authService;
    }

    public async Task<IActionResult> Index()
    {
        var students = await _studentRepository.GetAllStudentsAsync();
        return View(students);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var student = await _studentRepository.GetStudentDetailsByIdAsync(id);

            if (student == null)
                return NotFound("Student not found.");

            var borrowedCount = await _studentRepository.GetBorrowedBooksCountAsync(id);

            var model = new AdminStudentDetailsDto
            {
                Student = student,
                BorrowedBooksCount = borrowedCount,
                ReservedBooksCount = await _studentRepository.GetActiveReservationsCountAsync(id),

                // Manual fine rule: 1 borrowed book = Rs. 200
                PendingFines = borrowedCount * 200,

                BorrowedRecords = await _studentRepository.GetBorrowedItemsByStudentIdAsync(id),
                Reservations = await _studentRepository.GetActiveReservationsByStudentIdAsync(id)
            };

            return PartialView("_StudentDetailsPartial", model);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in Details Action: {ex}");
            return StatusCode(500, ex.ToString());
        }
    }
    [HttpPost]
    public async Task<IActionResult> AcceptBorrow(int recordId)
    {
        var result = await _studentRepository.AcceptBorrowRequestAsync(recordId);

        return Json(new
        {
            success = result,
            message = result ? "Borrow request accepted successfully." : "Failed to accept request."
        });
    }

    public async Task<IActionResult> GenerateChallan(int studentId)
    {
        var borrowedCount = await _studentRepository.GetBorrowedBooksCountAsync(studentId);
        var fineAmount = borrowedCount * 200;

        return RedirectToAction("Create", "Challan", new
        {
            studentId = studentId,
            amount = fineAmount
        });
    }
    [HttpPost]
    public async Task<IActionResult> ToggleBan(int studentId, string? reason)
    {
        if (studentId <= 0)
            return Json(new { success = false, message = "Invalid student ID." });

        var result = await _studentRepository.ToggleStudentBanAsync(studentId, reason);
        if (!result)
            return Json(new { success = false, message = "Student record was not found." });

        // Fetch the updated student to get the new IsBanned value
        var student = await _studentRepository.GetStudentByIdAsync(studentId);
        if (student == null)
            return Json(new { success = false, message = "Student not found after toggle." });

        return Json(new
        {
            success = true,
            message = "Student status updated successfully.",
            isBanned = student.IsBanned   // ← this is essential
        });
    }
    [HttpGet]
    public async Task<IActionResult> GetEditPartial(int id)
    {
        var student = await _studentRepository.GetStudentByIdAsync(id);

        if (student == null)
            return NotFound("Student not found.");

        return PartialView("_EditStudentPartial", student);
    }

    [HttpPost]
    public async Task<IActionResult> EditStudent(Student student, IFormFile? profileImageFile)
    {
        var existingStudent = await _studentRepository.GetStudentByIdAsync(student.StudentId);

        if (existingStudent == null)
            return NotFound("Student not found.");

        existingStudent.StudentName = student.StudentName;
        existingStudent.Username = student.Username;
        existingStudent.Email = student.Email;
        existingStudent.PhoneNumber = student.PhoneNumber;
        existingStudent.RollNo = student.RollNo;
        existingStudent.Department = student.Department;
        existingStudent.Batch = student.Batch;

        if (profileImageFile != null && profileImageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(profileImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImageFile.CopyToAsync(stream);
            }

            existingStudent.ImageUrl = "/uploads/students/" + fileName;
        }

        var result = await _studentRepository.UpdateStudentAsync(existingStudent);

        return Json(new
        {
            success = result,
            message = result ? "Student updated successfully." : "Failed to update student.",
            studentName = existingStudent.StudentName
        });
    }
    [HttpGet]
    public IActionResult GetCreateStudentPartial()
    {
        return PartialView("_CreateStudentPartial", new Student());
    }
    [HttpPost]
    public async Task<IActionResult> CreateStudent(
        Student student,
        string password,
        string confirmPassword,
        IFormFile? profileImageFile)
    {
        if (string.IsNullOrWhiteSpace(student.StudentName) ||
            string.IsNullOrWhiteSpace(student.Username) ||
            string.IsNullOrWhiteSpace(student.Email) ||
            string.IsNullOrWhiteSpace(student.RollNo) ||
            string.IsNullOrWhiteSpace(student.Batch) ||
            string.IsNullOrWhiteSpace(student.Department))
        {
            return Json(new
            {
                success = false,
                message = "Please fill in all required student information."
            });
        }

        if (password != confirmPassword)
        {
            return Json(new
            {
                success = false,
                message = "Password and confirm password do not match."
            });
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            return Json(new
            {
                success = false,
                message = "Password must be at least 6 characters."
            });
        }

        student.Role = "Student";
        student.IsBanned = false;
        student.BanReason = null;

        // AuthService will hash this plain password before saving.
        student.PasswordHash = password;

        if (profileImageFile != null && profileImageFile.Length > 0)
        {
            var extension = Path.GetExtension(profileImageFile.FileName)
                .ToLowerInvariant();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(extension))
            {
                return Json(new
                {
                    success = false,
                    message = "Only JPG, JPEG, and PNG image files are allowed."
                });
            }

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "students"
            );

            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await profileImageFile.CopyToAsync(stream);

            student.ImageUrl = $"/uploads/students/{fileName}";
        }

        // This calls AuthService.RegisterStudentAsync:
        // duplicate checks + secure password hashing + repository save.
        var result = await _authService.RegisterStudentAsync(student);

        return Json(new
        {
            success = result,
            message = result
                ? "Student created successfully."
                : "Username or roll number already already exists.",
            studentName = student.StudentName
        });
    }
  
    [HttpGet]
    public async Task<IActionResult> UserManagementDetail(int id)
    {
        var student = await _studentRepository.GetStudentByIdAsync(id);

        if (student == null)
            return NotFound("Student not found.");

        var borrowedBooksCount = await _studentRepository.GetBorrowedBooksCountAsync(id);
        ViewBag.BorrowedBooksCount = borrowedBooksCount;

        return PartialView("UserManagementDetail", student);
    }
}