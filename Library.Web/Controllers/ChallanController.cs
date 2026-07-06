using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Library.Web.Controllers;

public class ChallanController : Controller
{
    private readonly IChallanRepository _challanRepository;
    private readonly IStudentRepository _studentRepository;

    public ChallanController(
        IChallanRepository challanRepository,
        IStudentRepository studentRepository)
    {
        _challanRepository = challanRepository;
        _studentRepository = studentRepository;
    }

    // GET: /Challan/Index?studentId=5
    [HttpGet]
    public async Task<IActionResult> Index(int studentId)
    {
        if (studentId <= 0)
            return BadRequest("Invalid student ID.");

        var student = await _studentRepository.GetStudentByIdAsync(studentId);

        if (student == null)
            return NotFound("Student not found.");

        var borrowedBooksCount = await _studentRepository.GetBorrowedBooksCountAsync(studentId);

        var model = new CreateChallanDto
        {
            StudentId = student.StudentId,
            StudentName = student.StudentName ?? string.Empty,
            RollNo = student.RollNo ?? string.Empty,
            Department = student.Department ?? string.Empty,
            Batch = student.Batch ?? string.Empty,

            IssueDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(7),

            BankName = "Bank of Punjab",
            AccountTitle = "USKT Library",
            AccountNo = "",

            BorrowedBooksCount = borrowedBooksCount,
            IssuedBooksCount = borrowedBooksCount,
            PurchasedBooksCount = 0,

            Status = "Unpaid",
            Items = new List<CreateChallanItemDto>
            {
                new CreateChallanItemDto
                {
                    Particulars = string.Empty,
                    Quantity = 1,
                    UnitPrice = 0,
                    Amount = 0
                }
            }
        };

        return View("Index", model);
    }

    // GET: /Challan/Create?studentId=5
    [HttpGet]
    public async Task<IActionResult> Create(int studentId)
    {
        return await Index(studentId);
    }

    // POST: /Challan/Generate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(CreateChallanDto model)
    {
        Student? student = null;

        if (model.StudentId <= 0)
        {
            ModelState.AddModelError("", "Invalid student.");
        }
        else
        {
            student = await _studentRepository.GetStudentByIdAsync(model.StudentId);

            if (student == null)
            {
                ModelState.AddModelError("", "Student was not found.");
            }
        }

        if (model.DueDate.Date < model.IssueDate.Date)
        {
            ModelState.AddModelError("", "Due date cannot be earlier than issue date.");
        }

        model.Items ??= new List<CreateChallanItemDto>();

        var validItems = model.Items
            .Where(item =>
                !string.IsNullOrWhiteSpace(item.Particulars) &&
                item.Quantity > 0 &&
                item.UnitPrice >= 0)
            .ToList();

        if (!validItems.Any())
        {
            ModelState.AddModelError("", "Add at least one valid charge item.");
        }

        foreach (var item in validItems)
        {
            item.Particulars = item.Particulars.Trim();
            item.Amount = item.Quantity * item.UnitPrice;
        }

        model.Items = validItems;
        model.TotalAmount = validItems.Sum(item => item.Amount);

        if (!ModelState.IsValid)
        {
            if (student != null)
            {
                model.StudentName = student.StudentName ?? string.Empty;
                model.RollNo = student.RollNo ?? string.Empty;
                model.Department = student.Department ?? string.Empty;
                model.Batch = student.Batch ?? string.Empty;
                model.BorrowedBooksCount =
                    await _studentRepository.GetBorrowedBooksCountAsync(model.StudentId);
            }

            return View("Index", model);
        }

        var challan = new Challan
        {
            ChallanNo = await GenerateChallanNumberAsync(),
            StudentId = model.StudentId,

            IssueDate = model.IssueDate,
            DueDate = model.DueDate,

            BankName = model.BankName?.Trim() ?? string.Empty,
            AccountTitle = model.AccountTitle?.Trim() ?? string.Empty,
            AccountNo = model.AccountNo?.Trim() ?? string.Empty,

            BorrowedBooksCount = model.BorrowedBooksCount,
            IssuedBooksCount = model.IssuedBooksCount,
            PurchasedBooksCount = model.PurchasedBooksCount,

            TotalAmount = model.TotalAmount,
            Status = string.IsNullOrWhiteSpace(model.Status) ? "Unpaid" : model.Status,
            Notes = model.Notes?.Trim(),

            Items = validItems.Select(item => new ChallanItem
            {
                Particulars = item.Particulars,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Amount = item.Amount
            }).ToList()
        };

        var created = await _challanRepository.CreateChallanAsync(challan);

        if (!created)
        {
            ModelState.AddModelError("", "Challan could not be generated. Please try again.");
            return View("Index", model);
        }

        TempData["SuccessMessage"] = $"Challan {challan.ChallanNo} generated successfully.";

        return RedirectToAction(nameof(Print), new { id = challan.ChallanId });
    }

    // GET: /Challan/Details?id=5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var challan = await _challanRepository.GetChallanByIdAsync(id);

        if (challan == null)
            return NotFound("Challan not found.");

        return View(challan);
    }

    // GET: /Challan/Print?id=5
    [HttpGet]
    public async Task<IActionResult> Print(int id)
    {
        var challan = await _challanRepository.GetChallanByIdAsync(id);

        if (challan == null)
            return NotFound("Challan not found.");

        return View(challan);
    }

    // GET: /Challan/List
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var challans = await _challanRepository.GetAllChallansAsync();
        return View(challans);
    }

    // GET: /Challan/All
    [HttpGet]
    public async Task<IActionResult> All()
    {
        return RedirectToAction(nameof(List));
    }

    // POST: /Challan/UpdateStatus
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int challanId, string status)
    {
        var allowedStatuses = new[] { "Unpaid", "Partially Paid", "Paid", "Cancelled" };

        if (!allowedStatuses.Contains(status))
        {
            return Json(new
            {
                success = false,
                message = "Invalid challan status."
            });
        }

        var updated = await _challanRepository.UpdateChallanStatusAsync(challanId, status);

        return Json(new
        {
            success = updated,
            message = updated
                ? "Challan status updated successfully."
                : "Challan was not found."
        });
    }

    // POST: /Challan/Delete
    [HttpPost]
    public async Task<IActionResult> Delete(int challanId)
    {
        var deleted = await _challanRepository.DeleteChallanAsync(challanId);

        return Json(new
        {
            success = deleted,
            message = deleted
                ? "Challan deleted successfully."
                : "Challan was not found."
        });
    }

    private async Task<string> GenerateChallanNumberAsync()
    {
        var year = DateTime.Now.Year;
        var allChallans = await _challanRepository.GetAllChallansAsync();

        var countThisYear = allChallans.Count(challan =>
            challan.IssueDate.Year == year);

        return $"CH-{year}-{(countThisYear + 1):D4}";
    }
    // GET: /Challan/MyChallans
    [HttpGet]
    public async Task<IActionResult> MyChallans()
    {
        int? currentStudentId = HttpContext.Session.GetInt32("CurrentStudentId");

        if (currentStudentId == null)
            return RedirectToAction("Login", "Account");

        var challans = await _challanRepository
            .GetChallansByStudentIdAsync(currentStudentId.Value);

        ViewData["ActivePage"] = "MyChallans";

        ViewBag.UnpaidChallanCount =
            await _challanRepository
                .GetUnpaidChallanCountByStudentIdAsync(currentStudentId.Value);

        return View(challans);
    }
    // GET: /Challan/StudentPrint?id=5
    [HttpGet]
    public async Task<IActionResult> StudentPrint(int id)
    {
        int? currentStudentId = HttpContext.Session.GetInt32("CurrentStudentId");

        if (currentStudentId == null)
            return RedirectToAction("Login", "Account");

        var challan = await _challanRepository.GetChallanByIdAsync(id);

        if (challan == null)
            return NotFound("Challan not found.");

        if (challan.StudentId != currentStudentId.Value)
            return Unauthorized();

        return View("StudentPrint", challan);
    }
    [HttpPost]
    public async Task<IActionResult> NotifyStudent(int challanId)
    {
        var challan = await _challanRepository.GetChallanByIdAsync(challanId);

        if (challan == null)
        {
            return Json(new
            {
                success = false,
                message = "Challan not found."
            });
        }

        // Later you can save real notification in database/email.
        // For now student will see challan in My Challans automatically.

        return Json(new
        {
            success = true,
            message = "Student notified successfully.",
            studentId = challan.StudentId
        });
    }
    [HttpGet]
    public async Task<IActionResult> CreateFromDirect()
    {
        var studentIdText = TempData["DirectStudentId"]?.ToString();

        if (!int.TryParse(studentIdText, out var studentId) || studentId <= 0)
            return RedirectToAction("Index", "UserManagement");

        var student = await _studentRepository.GetStudentByIdAsync(studentId);

        if (student == null)
            return NotFound("Student not found.");

        var itemTitle = TempData["DirectItemTitle"]?.ToString() ?? "Library Item";
        var transactionType = TempData["DirectTransactionType"]?.ToString() ?? "Borrow";

        int.TryParse(TempData["DirectQuantity"]?.ToString(), out var quantity);
        if (quantity <= 0) quantity = 1;

        decimal.TryParse(
            TempData["DirectPrice"]?.ToString(),
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out var price);

        decimal.TryParse(
            TempData["DirectFineAmount"]?.ToString(),
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out var fineAmount);

        DateTime.TryParse(TempData["DirectIssueDate"]?.ToString(), out var issueDate);
        if (issueDate == default) issueDate = DateTime.Today;

        DateTime.TryParse(TempData["DirectDueDate"]?.ToString(), out var dueDate);
        if (dueDate == default) dueDate = DateTime.Today.AddDays(7);

        var items = new List<CreateChallanItemDto>();

        if (price > 0)
        {
            items.Add(new CreateChallanItemDto
            {
                Particulars = transactionType == "Sell"
                    ? $"Book Purchase: {itemTitle}"
                    : $"Borrowing Charges: {itemTitle}",
                Quantity = quantity,
                UnitPrice = price,
                Amount = quantity * price
            });
        }

        if (fineAmount > 0)
        {
            items.Add(new CreateChallanItemDto
            {
                Particulars = "Library Fine",
                Quantity = 1,
                UnitPrice = fineAmount,
                Amount = fineAmount
            });
        }

        if (!items.Any())
        {
            items.Add(new CreateChallanItemDto
            {
                Particulars = transactionType == "Sell"
                    ? $"Book Purchase: {itemTitle}"
                    : $"Borrowed Book: {itemTitle}",
                Quantity = quantity,
                UnitPrice = 0,
                Amount = 0
            });
        }

        var model = new CreateChallanDto
        {
            StudentId = student.StudentId,
            StudentName = student.StudentName ?? "",
            RollNo = student.RollNo ?? "",
            Department = student.Department ?? "",
            Batch = student.Batch ?? "",

            IssueDate = issueDate,
            DueDate = dueDate,

            BankName = "Bank of Punjab",
            AccountTitle = "USKT Library",
            AccountNo = "",

            BorrowedBooksCount = transactionType == "Borrow" ? quantity : 0,
            IssuedBooksCount = transactionType == "Borrow" ? quantity : 0,
            PurchasedBooksCount = transactionType == "Sell" ? quantity : 0,

            Status = "Unpaid",
            Notes = TempData["DirectNotes"]?.ToString(),
            Items = items,
            TotalAmount = items.Sum(x => x.Amount)
        };

        return View("Index", model);
    }

}