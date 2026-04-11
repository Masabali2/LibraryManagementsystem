namespace Library.Web.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Stats Cards
        public int TotalBooks { get; set; }
        public int ActiveStudents { get; set; }
        public int BooksIssuedToday { get; set; }
        public int OverdueReturns { get; set; }

        // Table Data
        public List<TransactionDto> RecentTransactions { get; set; }
    }

    public class TransactionDto
    {
        public string StudentName { get; set; }
        public string BookId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } // "Returned" or "Issued"
    }
}

