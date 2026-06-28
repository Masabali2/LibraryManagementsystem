using Microsoft.AspNetCore.Mvc;
using Library.Domain.Interfaces;
using System.Threading.Tasks;

namespace Library.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookRepository _bookRepository;

        // Inject the interface here
        public HomeController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch 4 books from the database via your repository layer
            var featuredBooks = await _bookRepository.GetFeaturedBooksAsync(4);

            // Pass the data list to the Index.cshtml view
            return View(featuredBooks);
        }
    }
}