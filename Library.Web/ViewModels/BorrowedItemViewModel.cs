namespace Library.Web.ViewModels
{
    public class BorrowedItemViewModel
    {
        public string Title { get; set; } = string.Empty;
        
        public string Type { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public DateTime BorrowedDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsDueSoon { get; set; }
        public string Status { get; set; } = null!; // 🚀 New: "Pending", "Approved"
        public string RequestType { get; set; } = null!;


    }
}