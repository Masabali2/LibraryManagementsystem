using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Library.Web.Controllers;

public class BorrowingController : Controller
{
    private readonly IStudentRepository _studentRepository;
    private readonly IBookRepository _bookRepository;

    public BorrowingController(
        IStudentRepository studentRepository,
        IBookRepository bookRepository)
    {
        _studentRepository = studentRepository;
        _bookRepository = bookRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Direct(int studentId)
    {
        if (studentId <= 0)
            return BadRequest("Invalid student ID.");

        var student = await _studentRepository.GetStudentByIdAsync(studentId);

        if (student == null)
            return NotFound("Student not found.");

        var model = new DirectBorrowDto
        {
            StudentId = student.StudentId,
            StudentName = student.StudentName ?? string.Empty,
            RollNo = student.RollNo ?? string.Empty,
            Department = student.Department ?? string.Empty,
            Batch = student.Batch ?? string.Empty,

            TransactionType = "Borrow",
            IssueDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(14),

            Quantity = 1,
            Price = 0,
            FineAmount = 0,

            // The Direct.cshtml hidden input posts true.
            GenerateChallan = true,

            AvailableItems = await _bookRepository.GetAvailableLibraryItemsAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Direct(DirectBorrowDto model)
    {
        var student = await _studentRepository.GetStudentByIdAsync(model.StudentId);

        if (student == null)
            return NotFound("Student not found.");

        if (model.TransactionType != "Borrow" && model.TransactionType != "Sell")
            ModelState.AddModelError("", "Transaction type must be Borrow or Sell.");

        if (model.ItemId <= 0)
            ModelState.AddModelError("", "Please select a book or library item.");

        if (model.Quantity <= 0)
            ModelState.AddModelError("", "Quantity must be at least 1.");

        if (model.Price < 0 || model.FineAmount < 0)
            ModelState.AddModelError("", "Price and fine cannot be negative.");

        if (model.TransactionType == "Borrow")
        {
            if (!model.DueDate.HasValue)
            {
                ModelState.AddModelError("", "Due date is required for a borrowed item.");
            }
            else if (model.DueDate.Value.Date < model.IssueDate.Date)
            {
                ModelState.AddModelError("", "Due date cannot be earlier than issue date.");
            }
        }

        // Never trust ItemTitle from hidden HTML input.
        // Read the selected item from the database.
        var availableItems = await _bookRepository.GetAvailableLibraryItemsAsync();
        var selectedItem = availableItems.FirstOrDefault(item => item.ItemId == model.ItemId);

        if (selectedItem == null)
        {
            ModelState.AddModelError("", "The selected book is unavailable or does not exist.");
        }
        else
        {
            model.ItemTitle = selectedItem.Title;
            model.ItemType = selectedItem.ItemType;
        }

        model.TotalAmount = (model.Price * model.Quantity) + model.FineAmount;

        if (!ModelState.IsValid)
        {
            model.StudentName = student.StudentName ?? string.Empty;
            model.RollNo = student.RollNo ?? string.Empty;
            model.Department = student.Department ?? string.Empty;
            model.Batch = student.Batch ?? string.Empty;
            model.AvailableItems = availableItems;

            return View(model);
        }

        TempData["DirectStudentId"] = model.StudentId.ToString();
        TempData["DirectItemId"] = model.ItemId.ToString();
        TempData["DirectItemTitle"] = model.ItemTitle ?? "";
        TempData["DirectItemType"] = model.ItemType ?? "Book";
        TempData["DirectTransactionType"] = model.TransactionType ?? "Borrow";

        TempData["DirectIssueDate"] = model.IssueDate.ToString("yyyy-MM-dd");
        TempData["DirectDueDate"] = model.DueDate?.ToString("yyyy-MM-dd") ?? "";

        TempData["DirectQuantity"] = model.Quantity.ToString();
        TempData["DirectPrice"] = model.Price.ToString(CultureInfo.InvariantCulture);
        TempData["DirectFineAmount"] = model.FineAmount.ToString(CultureInfo.InvariantCulture);
        TempData["DirectNotes"] = model.Notes ?? "";

        return RedirectToAction("CreateFromDirect", "Challan");

    }
}