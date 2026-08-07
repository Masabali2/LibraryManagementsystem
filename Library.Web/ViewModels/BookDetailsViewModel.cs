namespace Library.Web.ViewModels
{
    public class BookDetailsViewModel
    {
        public Library.Domain.Entities.Book Book { get; set; } = null!;
        public string? UserActiveStatus { get; set; }
        public bool IsAlreadyBorrowedOrReserved { get; set; }
    }
}
