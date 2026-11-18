using System;
using System.Collections.Generic;

namespace LibraryWithSql_PO.models
{
    public partial class Category
    {
        public int CategoryID { get; set; } // מפתח ראשי 
        public string CategoryName { get; set; } 
        public string? Description { get; set; } 

        public virtual ICollection<Book> Books { get; set; } 
    }
}