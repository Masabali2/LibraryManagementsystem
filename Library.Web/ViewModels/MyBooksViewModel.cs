namespace Library.Web.ViewModels
{
    public class MyBooksViewModel
    {
        public List<ActiveBorrowedBookViewModel> ActiveBorrowedBooks { get; set; } = new List<ActiveBorrowedBookViewModel>();
        public List<PastReadViewModel> PastReads { get; set; } = new List<PastReadViewModel>();
    }

    public class ActiveBorrowedBookViewModel
    {
        public int BorrowingRecordId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Type { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
    }

    public class PastReadViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public string Type { get; set; }
        public DateTime? ActualReturnDate { get; set; }
    }
}
