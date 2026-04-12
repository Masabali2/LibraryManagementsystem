using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly LibraryDbContext _context;

    public InventoryRepository(LibraryDbContext context)
    {
        _context = context;
    }

    // --- FETCHING BY ID (Required for the Edit Modal) ---

    public async Task<List<Book>> GetAllBooksAsync()
        => await _context.Books.Include(b => b.Shelf).ToListAsync();

    public async Task<List<Thesis>> GetAllThesesAsync()
        => await _context.Theses.Include(t => t.Shelf).ToListAsync();

    public async Task<List<Journal>> GetAllJournalsAsync()
        => await _context.Journals.Include(j => j.Shelf).ToListAsync();
    public async Task<Book?> GetBookByIdAsync(int id)
        => await _context.Books.Include(b => b.Shelf).FirstOrDefaultAsync(b => b.BookId == id);

    public async Task<Thesis?> GetThesisByIdAsync(int id)
        => await _context.Theses.Include(t => t.Shelf).FirstOrDefaultAsync(t => t.ThesisId == id);

    public async Task<Journal?> GetJournalByIdAsync(int id)
        => await _context.Journals.Include(j => j.Shelf).FirstOrDefaultAsync(j => j.JournalId == id);

    // --- UPDATE METHODS (Required for Saving Changes) ---
    // --- HELPER: Logic to handle manual text and convert to Database IDs ---
    private async Task<int?> GetOrCreateShelfIdAsync(string? blockName, string? shelfCode)
    {
        if (string.IsNullOrWhiteSpace(blockName) || string.IsNullOrWhiteSpace(shelfCode))
            return null;

        // 1. Find or create the Location Block
        var block = await _context.LocationBlocks
            .FirstOrDefaultAsync(b => b.Name.Trim().ToLower() == blockName.Trim().ToLower());

        if (block == null)
        {
            block = new LocationBlock { Name = blockName.Trim() };
            _context.LocationBlocks.Add(block);
            await _context.SaveChangesAsync();
        }

        // 2. Find or create the Shelf within that block
        var shelf = await _context.Shelves
            .FirstOrDefaultAsync(s => s.ShelfCode.Trim().ToLower() == shelfCode.Trim().ToLower()
                                    && s.LocationBlockId == block.LocationBlockId);

        if (shelf == null)
        {
            shelf = new Shelf { ShelfCode = shelfCode.Trim(), LocationBlockId = block.LocationBlockId };
            _context.Shelves.Add(shelf);
            await _context.SaveChangesAsync();
        }

        return shelf.ShelfId;
    }

    // --- UPDATED UPDATE METHODS ---

    public async Task<bool> UpdateBookAsync(Book book, string? blockName, string? shelfCode)
    {
        book.ShelfId = await GetOrCreateShelfIdAsync(blockName, shelfCode);
        _context.Books.Update(book);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateThesisAsync(Thesis thesis, string? blockName, string? shelfCode)
    {
        thesis.ShelfId = await GetOrCreateShelfIdAsync(blockName, shelfCode);
        _context.Theses.Update(thesis);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateJournalAsync(Journal journal, string? blockName, string? shelfCode)
    {
        journal.ShelfId = await GetOrCreateShelfIdAsync(blockName, shelfCode);
        _context.Journals.Update(journal);
        return await _context.SaveChangesAsync() > 0;
    }

    // --- DELETION METHODS ---

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return false;
        _context.Books.Remove(book);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteThesisAsync(int id)
    {
        var thesis = await _context.Theses.FindAsync(id);
        if (thesis == null) return false;
        _context.Theses.Remove(thesis);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteJournalAsync(int id)
    {
        var journal = await _context.Journals.FindAsync(id);
        if (journal == null) return false;
        _context.Journals.Remove(journal);
        return await _context.SaveChangesAsync() > 0;
    }

    // --- UNIFIED INVENTORY VIEW ---

    public async Task<List<UnifiedInventoryDto>> GetUnifiedInventoryAsync()
    {
        var bookQuery = _context.Books.Select(b => new UnifiedInventoryDto
        {
            Id = b.BookId,
            Title = b.Title,
            Author = b.Author,
            Year = b.PublicationYear,
            TotalCopies = b.TotalCopies ?? 0,
            AvailableCopies = b.AvailableCopies ?? 0,
            Department = b.Department,
            Type = "book",
            ShelfCode = b.Shelf != null ? b.Shelf.ShelfCode : "N/A",
            BlockName = b.Shelf != null && b.Shelf.LocationBlock != null ? b.Shelf.LocationBlock.Name : "Unassigned"
        });

        var journalQuery = _context.Journals.Select(j => new UnifiedInventoryDto
        {
            Id = j.JournalId,
            Title = j.JournalName, // Assuming Title property from your entity
            Author = j.Publisher ?? "Unknown",
            Year = j.Year,
            TotalCopies = 1, // Adjust based on your Journal logic
            AvailableCopies = 1,
            Department = j.Department ?? "General",
            Type = "journal",
            ShelfCode = j.Shelf != null ? j.Shelf.ShelfCode : "N/A",
            BlockName = j.Shelf != null && j.Shelf.LocationBlock != null ? j.Shelf.LocationBlock.Name : "Unassigned"
        });

        var thesisQuery = _context.Theses.Select(t => new UnifiedInventoryDto
        {
            Id = t.ThesisId,
            Title = t.Title,
            Author = t.StudentName,
            Year = t.Year.ToString(),
            TotalCopies = 1,
            AvailableCopies = 1,
            Department = t.Department,
            Type = "thesis",
            ShelfCode = t.Shelf != null ? t.Shelf.ShelfCode : "N/A",
            BlockName = t.Shelf != null && t.Shelf.LocationBlock != null ? t.Shelf.LocationBlock.Name : "Unassigned"
        });

        return await bookQuery.Union(journalQuery).Union(thesisQuery).ToListAsync();
    }
    // --- CREATE METHODS (Add New Asset) ---

    public async Task<bool> AddBookAsync(Book book, string? blockName, string? shelfCode)
    {
        book.ShelfId = await GetOrCreateShelfIdAsync(blockName, shelfCode);
        await _context.Books.AddAsync(book);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddThesisAsync(Thesis thesis, string? blockName, string? shelfCode)
    {
        thesis.ShelfId = await GetOrCreateShelfIdAsync(blockName, shelfCode);
        await _context.Theses.AddAsync(thesis);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddJournalAsync(Journal journal, string? blockName, string? shelfCode)
    {
        journal.ShelfId = await GetOrCreateShelfIdAsync(blockName, shelfCode);
        await _context.Journals.AddAsync(journal);
        return await _context.SaveChangesAsync() > 0;
    }
}