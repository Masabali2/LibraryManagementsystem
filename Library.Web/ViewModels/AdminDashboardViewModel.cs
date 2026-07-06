using System;
using System.Collections.Generic;

namespace Library.Web.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Stats Cards - Main Totals
        public int TotalBooks { get; set; }
        public int ActiveStudents { get; set; }
        public int BooksIssuedToday { get; set; }
        public int OverdueReturns { get; set; }

        // Inventory Breakdown (Fixes the "definition not found" errors)
        public int BooksCount { get; set; }
        public int ThesesCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int JournalsCount { get; set; }

        // Table Data
        public List<TransactionDto> RecentTransactions { get; set; }
    }

    public class TransactionDto
    {
        public int RecordId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        // Generalizing from BookId to ItemId to cover Thesis/Journals
        public string ItemId { get; set; }

        // New property to show "Book", "Thesis", or "Journal" in the UI
        public string ItemName { get; set; }
        public string ItemType { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; } // "Returned" or "Issued"

        // Helper for backwards compatibility if you're using .BookId in other places
        public string BookId => ItemId;
    }
}