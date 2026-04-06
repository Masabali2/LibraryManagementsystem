using System;
using System.Collections.Generic;

namespace Library.Domain.Entities;

public class Thesis
{
    public int ThesisId { get; set; }
    public string Title { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string RollNo { get; set; } = null!;
    public string Batch { get; set; } = null!;
    public int Year { get; set; }
}