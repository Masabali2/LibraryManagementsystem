using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Domain.Entities;

public class Thesis
{
    public int ThesisId { get; set; }
    public string Department { get; set; } = null!;
    [StringLength(1000)]
    public string Title { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string RollNo { get; set; } = null!;
    public string Batch { get; set; } = null!;
    public int Year { get; set; }
    public int? ShelfId { get; set; } // Nullable so old data doesn't crash
    public virtual Shelf? Shelf { get; set; }
}