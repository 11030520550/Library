using System;
using System.Collections.Generic;

namespace LibraryWithSql_PO.models
{
    public partial class Loan
    {
        public int LoanID { get; set; } 
        public int UserID { get; set; } 
        public int BookID { get; set; } 
        public DateTime LoanDate { get; set; } = DateTime.Now;  
        public DateTime DueDate { get; set; } 
        public DateTime? ReturnDate { get; set; } 
        public string Status { get; set; } = "Active"; //  'Active', 'Returned', 'Overdue' 

        
        public virtual User User { get; set; } 
        public virtual Book Book { get; set; } 
    }
}