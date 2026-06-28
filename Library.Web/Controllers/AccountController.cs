using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IStudentRepository _studentRepo;

    public AccountController(IAuthService authService, IStudentRepository studentRepo)
    {
        _authService = authService;
        _studentRepo = studentRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // 1. Admin Authentication
        bool isAdmin = await _authService.AuthenticateAdminAsync(username, password);
        if (isAdmin)
        {
            HttpContext.Session.SetString("UserRole", "Admin");
            HttpContext.Session.SetString("Username", username);
            return RedirectToAction("Index", "Admin");
        }
        // 2. Student Authentication
        bool isStudent = await _authService.AuthenticateStudentAsync(username, password);
        if (isStudent)
        {
            var student = await _studentRepo.GetStudentByUsernameAsync(username);
            if (student != null)
            {
                HttpContext.Session.SetString("UserRole", "Student");
                HttpContext.Session.SetInt32("CurrentStudentId", student.StudentId);
                HttpContext.Session.SetString("StudentName", student.StudentName);
                HttpContext.Session.SetString("StudentImageUrl", student.ImageUrl ?? "");
            }

            return RedirectToAction("Dashboard", "Student");
        }

        ViewBag.ErrorMessage = "Invalid username or password!";
        return View();
    }

    [HttpGet]
    public IActionResult Register() => View();
    [HttpPost]
    public async Task<IActionResult> Register(Student student, IFormFile? profileImageFile)
    {
        if (ModelState.IsValid)
        {
            // 🔒 FIX: Route this through your AuthService so it gets properly hashed and validated!
            bool isAdded = await _authService.RegisterStudentAsync(student);

            if (isAdded)
            {
                // Now that it's successfully added, handle the profile image if there is one
                if (profileImageFile != null && profileImageFile.Length > 0)
                {
                    // student.StudentId is now populated by EF Core after the database save
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", student.StudentId.ToString());

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string extension = Path.GetExtension(profileImageFile.FileName);
                    string fileName = $"profile_image{extension}";
                    string fullPhysicalPath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(fullPhysicalPath, FileMode.Create))
                    {
                        await profileImageFile.CopyToAsync(stream);
                    }

                    // Update path reference field (Make sure your property matches your entity, e.g., ImageUrl or ProfileImageUrl)
                    student.ImageUrl = $"/images/{student.StudentId}/{fileName}";

                    // Update the student row with the image path link
                    await _studentRepo.UpdateStudentAsync(student);
                }

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            // If isAdded is false, it means the username or Roll Number already existed
            ModelState.AddModelError("", "Username or Roll Number is already registered.");
        }

        return View(student);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}