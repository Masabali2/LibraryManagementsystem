using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting; // 🚀 Required for file environment paths
using Microsoft.AspNetCore.Http;    // 🚀 Required for IFormFile

namespace Library.Web.Controllers;

public class InventoryController : Controller
{
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IWebHostEnvironment _webHostEnvironment; // 🚀 Private field injected

    public InventoryController(IInventoryRepository inventoryRepo, IWebHostEnvironment webHostEnvironment)
    {
        _inventoryRepo = inventoryRepo;
        _webHostEnvironment = webHostEnvironment; // 🚀 Assigned to local property
    }

    public async Task<IActionResult> Index()
    {
        var allItems = await _inventoryRepo.GetUnifiedInventoryAsync();

        var groupedInventory = allItems
            .GroupBy(i => !string.IsNullOrWhiteSpace(i.Department) ? i.Department.Trim() : "General/Unassigned")
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());

        return View(groupedInventory);
    }

    [HttpGet]
    public async Task<IActionResult> GetEditPartial(int id, string type)
    {
        if (id <= 0 || string.IsNullOrEmpty(type)) return BadRequest();

        return type.ToLower() switch
        {
            "book" => PartialView("_EditBookPartial", await _inventoryRepo.GetBookByIdAsync(id)),
            "thesis" => PartialView("_EditThesisPartial", await _inventoryRepo.GetThesisByIdAsync(id)),
            "journal" => PartialView("_EditJournalPartial", await _inventoryRepo.GetJournalByIdAsync(id)),
            _ => NotFound()
        };
    }

    // --- UPDATE ACTIONS ---
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBook(Book book, string LocationBlockName, string ShelfCode, IFormFile? BookImage)
    {
        // 1. Fetch the existing entity from the database
        var existingBook = await _inventoryRepo.GetBookByIdAsync(book.BookId);
        if (existingBook == null)
        {
            return NotFound();
        }

        // 2. Handle Image Logic
        if (BookImage != null && BookImage.Length > 0)
        {
            // If there was an old image, delete it from physical storage
            if (!string.IsNullOrEmpty(existingBook.ImageUrl))
            {
                DeleteExistingFile(existingBook.ImageUrl);
            }

            // Save new image and update the URL property
            // This will create a path like: /images/books/book_5.jpg
            existingBook.ImageUrl = await SaveFileAsync(BookImage, "books", "book", book.BookId);
        }
        
        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.Department = book.Department;
        existingBook.PublicationYear = book.PublicationYear;
        existingBook.TotalCopies = book.TotalCopies;
        existingBook.AvailableCopies = book.AvailableCopies;

        // 4. Update the record via the repository
        await _inventoryRepo.UpdateBookAsync(existingBook, LocationBlockName, ShelfCode);

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateThesis(Thesis thesis, string LocationBlockName, string ShelfCode, IFormFile? ThesisImage)
    {
        // 1. Fetch the existing record to maintain database tracking
        var existingThesis = await _inventoryRepo.GetThesisByIdAsync(thesis.ThesisId);
        if (existingThesis == null) return NotFound();

        // 2. Handle Image: Replace only if a new file is uploaded
        if (ThesisImage != null && ThesisImage.Length > 0)
        {
            if (!string.IsNullOrEmpty(existingThesis.ImageUrl))
            {
                DeleteExistingFile(existingThesis.ImageUrl);
            }
            existingThesis.ImageUrl = await SaveFileAsync(ThesisImage, "thesis", "thesis", thesis.ThesisId);
        }

        // 3. Update properties from the form model
        existingThesis.Title = thesis.Title;
        existingThesis.Year = thesis.Year;
        existingThesis.StudentName = thesis.StudentName;
        existingThesis.RollNo = thesis.RollNo;
        existingThesis.Department = thesis.Department;
        existingThesis.Batch = thesis.Batch;

        // 4. Update the record
        await _inventoryRepo.UpdateThesisAsync(existingThesis, LocationBlockName, ShelfCode);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateJournal(Journal journal, string LocationBlockName, string ShelfCode, IFormFile? JournalImage)
    {
        // 1. Fetch the existing record
        var existingJournal = await _inventoryRepo.GetJournalByIdAsync(journal.JournalId);
        if (existingJournal == null) return NotFound();

        // 2. Handle Image: Replace only if a new file is uploaded
        if (JournalImage != null && JournalImage.Length > 0)
        {
            if (!string.IsNullOrEmpty(existingJournal.ImageUrl))
            {
                DeleteExistingFile(existingJournal.ImageUrl);
            }
            existingJournal.ImageUrl = await SaveFileAsync(JournalImage, "journal", "journal", journal.JournalId);
        }

        // 3. Update properties from the form model
        existingJournal.JournalName = journal.JournalName;
        existingJournal.Year = journal.Year;
        existingJournal.Publisher = journal.Publisher;
        existingJournal.Department = journal.Department;
        existingJournal.Volume = journal.Volume;

        // 4. Update the record
        await _inventoryRepo.UpdateJournalAsync(existingJournal, LocationBlockName, ShelfCode);

        return RedirectToAction(nameof(Index));
    }

    // --- DELETE ACTION ---
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> Delete(int id, string type)
    {
        if (id <= 0 || string.IsNullOrWhiteSpace(type))
        {
            return Json(new { success = false, message = "Invalid request parameters." });
        }

        bool result;
        try
        {
            result = type.ToLower() switch
            {
                "book" => await _inventoryRepo.DeleteBookAsync(id),
                "thesis" => await _inventoryRepo.DeleteThesisAsync(id),
                "journal" => await _inventoryRepo.DeleteJournalAsync(id),
                _ => false
            };

            if (result)
            {
                return Json(new { success = true, message = "Deleted successfully." });
            }
            return Json(new { success = false, message = "Item not found." });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "A server error occurred." });
        }
    }

    // GET: Return empty create partials
    [HttpGet]
    public IActionResult GetCreatePartial(string type)
    {
        return type?.ToLower() switch
        {
            "book" => PartialView("_CreateBookPartial", new Book()),
            "thesis" => PartialView("_CreateThesisPartial", new Thesis()),
            "journal" => PartialView("_CreateJournalPartial", new Journal()),
            _ => BadRequest()
        };
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBook(Book book, string LocationBlockName, string ShelfCode, IFormFile? BookImage)
    {
        if (!ModelState.IsValid) return PartialView("_CreateBookPartial", book);

        await _inventoryRepo.AddBookAsync(book, LocationBlockName, ShelfCode);

        if (BookImage != null && BookImage.Length > 0)
        {
            book.ImageUrl = await SaveFileAsync(BookImage, "books", "book", book.BookId);
            await _inventoryRepo.UpdateBookAsync(book, LocationBlockName, ShelfCode);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateThesis(Thesis thesis, string LocationBlockName, string ShelfCode, IFormFile? ThesisImage)
    {
        if (!ModelState.IsValid) return PartialView("_CreateThesisPartial", thesis);

        await _inventoryRepo.AddThesisAsync(thesis, LocationBlockName, ShelfCode);

        if (ThesisImage != null && ThesisImage.Length > 0)
        {
            thesis.ImageUrl = await SaveFileAsync(ThesisImage, "thesis", "thesis", thesis.ThesisId);
            await _inventoryRepo.UpdateThesisAsync(thesis, LocationBlockName, ShelfCode);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateJournal(Journal journal, string LocationBlockName, string ShelfCode, IFormFile? JournalImage)
    {
        if (!ModelState.IsValid) return PartialView("_CreateJournalPartial", journal);

        await _inventoryRepo.AddJournalAsync(journal, LocationBlockName, ShelfCode);

        if (JournalImage != null && JournalImage.Length > 0)
        {
            journal.ImageUrl = await SaveFileAsync(JournalImage, "journal", "journal", journal.JournalId);
            await _inventoryRepo.UpdateJournalAsync(journal, LocationBlockName, ShelfCode);
        }
        return RedirectToAction(nameof(Index));
    }
    private void DeleteExistingFile(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;

        // Convert the web URL path back to a physical server path
        string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/'));

        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
    }

    private async Task<string> SaveFileAsync(IFormFile file, string subFolder, string prefix, int id)
    {
        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", subFolder);

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string extension = Path.GetExtension(file.FileName).ToLower();

        string fileName = $"{prefix}_{id}{extension}";
        string filePath = Path.Combine(uploadsFolder, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/images/{subFolder}/{fileName}";
    }
    // Add this to your controller
    public IActionResult GetInventoryListPartial()
    {
        // Reuse your logic that gets the data for the main view
        var data = _inventoryRepo.GetUnifiedInventoryAsync();
        return PartialView("_InventoryListPartial", data);
    }
}