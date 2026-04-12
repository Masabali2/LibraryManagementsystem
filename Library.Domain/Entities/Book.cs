    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    namespace Library.Domain.Entities;

    public class Book
    {
        public int BookId { get; set; }
        public string Department { get; set; }
        [StringLength(500)]
        public string Title { get; set; } = null!;
        [StringLength(500)]
        public string Author { get; set; } = null!;
        [StringLength(100)]
        public string? PublicationYear { get; set; }
        [StringLength(50)]
        public string? Edition { get; set; }
        public int? TotalCopies { get; set; }
        public int? AvailableCopies { get; set; }
    public int? ShelfId { get; set; } // Nullable so old data doesn't crash
    public virtual Shelf? Shelf { get; set; }
}
