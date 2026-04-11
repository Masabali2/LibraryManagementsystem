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
            }

            return RedirectToAction("Dashboard", "Student");
        }

        ViewBag.ErrorMessage = "Invalid username or password!";
        return View();
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(Student student)
    {
        if (ModelState.IsValid)
        {
            bool isRegistered = await _authService.RegisterStudentAsync(student);
            if (isRegistered) return RedirectToAction("Login");

            ViewBag.ErrorMessage = "Username or Roll Number already registered!";
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