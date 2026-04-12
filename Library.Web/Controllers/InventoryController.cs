using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class InventoryController : Controller
{
    private readonly IInventoryRepository _inventoryRepo;

    public InventoryController(IInventoryRepository inventoryRepo)
    {
        _inventoryRepo = inventoryRepo;
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

        // We fetch by ID. The repository's 'Include' logic will load the Shelf and Block names 
        // which will automatically populate the manual text boxes in our Partial Views.
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
    public async Task<IActionResult> UpdateBook(Book book, string LocationBlockName, string ShelfCode)
    {
        // Pass all 3 arguments to fix the "no argument given" error
        await _inventoryRepo.UpdateBookAsync(book, LocationBlockName, ShelfCode);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateThesis(Thesis thesis, string LocationBlockName, string ShelfCode)
    {
        await _inventoryRepo.UpdateThesisAsync(thesis, LocationBlockName, ShelfCode);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateJournal(Journal journal, string LocationBlockName, string ShelfCode)
    {
        await _inventoryRepo.UpdateJournalAsync(journal, LocationBlockName, ShelfCode);
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
}