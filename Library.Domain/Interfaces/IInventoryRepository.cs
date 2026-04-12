using Library.Domain.Entities;
using Library.Domain.DTOs;

namespace Library.Domain.Interfaces;

public interface IInventoryRepository
{
    // --- UNIFIED VIEW ---
    Task<List<UnifiedInventoryDto>> GetUnifiedInventoryAsync();

   
    Task<Book?> GetBookByIdAsync(int id);
    Task<Thesis?> GetThesisByIdAsync(int id);
    Task<Journal?> GetJournalByIdAsync(int id);

    Task<bool> UpdateBookAsync(Book book, string? blockName, string? shelfCode);
    Task<bool> UpdateThesisAsync(Thesis thesis, string? blockName, string? shelfCode);
    Task<bool> UpdateJournalAsync(Journal journal, string? blockName, string? shelfCode);

    // --- DELETION METHODS ---
    Task<bool> DeleteBookAsync(int id);
    Task<bool> DeleteThesisAsync(int id);
    Task<bool> DeleteJournalAsync(int id);

    // --- BULK FETCH (Optional/Legacy) ---
    Task<List<Book>> GetAllBooksAsync();
    Task<List<Thesis>> GetAllThesesAsync();
    Task<List<Journal>> GetAllJournalsAsync();
}