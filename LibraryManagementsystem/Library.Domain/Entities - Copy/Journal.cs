using System;
using System.Collections.Generic;
namespace Library.Domain.Entities;

public class Journal
{
    public int JournalId { get; set; }
    public string JournalName { get; set; } = null!;
    public string? Department { get; set; }
    public string? Publisher { get; set; }
    public int? Volume { get; set; }
    public int? Edition { get; set; }
    public int Year { get; set; }
    public int? Quantity { get; set; }
}