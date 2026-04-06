//using Microsoft.AspNetCore.Mvc;
//using LibraryManagementsystem.Models;
//using System.Linq;

//namespace LibraryManagementsystem.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly librarydbContext _context;

//        public AccountController(librarydbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Login(string username, string password)
//        {
           
//            var adminInDb = _context.Admins
//                .Where(u => u.Username == username && u.PasswordHash == password)
//                .FirstOrDefault();
//            if (adminInDb != null)
//            {
                
//                return RedirectToAction("Index", "Home");
//            }
//            ViewBag.ErrorMessage = "Access Denied: Invalid credentials found in the database.";
//            return View();
//        }
//    }
//}