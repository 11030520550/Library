using System;
using System.Collections.Generic;

namespace LibraryWithSql_PO.models
{
    public partial class Book
    {
        // הערה: partial class נשאר
        public int BookID { get; set; }
        public int BookId { get; internal set; }
        public string Title { get; set; } 
        public string Author { get; set; } 
        public int? PublishedYear { get; set; } 
        public int? CategoryID { get; set; } 
        public string? Summary { get; set; } 
        public string? ISBN { get; set; } 
        public string? AvailabilityStatus { get; set; } 
        public DateTime? AddedDate { get; set; } 

        public string? PreviewContent { get; set; } 
        public int TotalCopies { get; set; } = 1; 
        public int AvailableCopies { get; set; } = 1; 
        public string? CoverImageUrl { get; set; } 

        public virtual Category Category { get; set; } 
        public virtual ICollection<Loan> Loans { get; set; } 
}
    }