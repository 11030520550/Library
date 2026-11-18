using System;
using System;
using System.Collections.Generic;

namespace LibraryWithSql_PO.models
{
    public partial class User
    {
        public int UserID { get; set; } 
        public string FullName { get; set; } 
        public string Email { get; set; }
        public string PasswordHash { get; set; } 
        public string Role { get; set; } = "Member"; 
        public string Status { get; set; } = "Pending"; 
        public DateTime RegistrationDate { get; set; } 
        public bool IsAdmin { get; set; } 

        public string? SubscriptionType { get; set; } = "None"; // 'Monthly', 'PayPerBook', 'None' [cite: 147]
        public DateTime? SubscriptionEndDate { get; set; } 

        // קשרים לטבלאות אחרות (Navigation Properties):
        public virtual ICollection<Loan> Loans { get; set; } // רשימת השאלות של המשתמש 
    }
}