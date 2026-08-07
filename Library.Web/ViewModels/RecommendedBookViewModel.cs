using Library.Domain.Entities;

namespace Library.Web.ViewModels;

public class RecommendedBookViewModel
{
    public Book Book { get; set; } = null!;

    public double Score { get; set; }

    public string Reason { get; set; } = "";
}      