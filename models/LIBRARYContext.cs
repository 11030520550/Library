using System;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace LibraryWithSql_PO.models
{
    public partial class LIBRARYContext : DbContext
    {
        public LIBRARYContext()
        {
        }

        public LIBRARYContext(DbContextOptions<LIBRARYContext> options)
            : base(options)
        {
        }

        // הגדרת ה-DbSets עבור הטבלאות (כולל החדשות)
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Category> Categories { get; set; } // טבלה חדשה
        public virtual DbSet<Loan> Loans { get; set; } // טבלה חדשה

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // כאן תדביקי את ה-Connection String שלך אם הוא לא מוגדר ב-appsettings.json
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=YOUR_SERVER_NAME;Database=LIBRARY;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelCreatingBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Hebrew_CI_AS");

            // הגדרות עבור טבלת Books
            modelbuilder.Entity<book>(entity =>
            {
                entity.HasKey(e => e.BookID);

                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Author).IsRequired().HasMaxLength(150);
                entity.Property(e => e.ISBN).HasMaxLength(20);
                entity.Property(e => e.CoverImageUrl).HasMaxLength(500);

                // הגדרת הקשר לקטגוריה (Many-to-One)
                // זה התיקון הקריטי! הקישור חייב להיותNavigation Property
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.CategoryID)
                    .HasConstraintName("FK__Books__CategoryI__...");
            });

            // הגדרות עבור טבלת Users
            modelbuilder.Entity<user>(entity =>
            {
                entity.HasKey(e => e.UserID);

                entity.Property(e => e.FullName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20).HasDefaultValueSql("'Member'");
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValueSql("'Pending'");
                entity.Property(e => e.SubscriptionType).HasMaxLength(20).HasDefaultValueSql("'None'");

                entity.HasIndex(e => e.Email).IsUnique(); // אינדקס ייחודי לאימייל
            });

            // הגדרות עבור טבלת Categories
            modelbuilder.Entity<category>(entity =>
            {
                entity.HasKey(e => e.CategoryID);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.HasIndex(e => e.CategoryName).IsUnique(); // שם קטגוריה ייחודי
            });

            // הגדרות עבור טבלת Loans
            modelbuilder.Entity<loan>(entity =>
            {
                entity.HasKey(e => e.LoanID);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValueSql("'Active'");

                // הגדרת הקשר למשתמש (Many-to-One)
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Loans__UserID__...");

                // הגדרת הקשר לספר (Many-to-One)
                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.BookID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Loans__BookID__...");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelCreatingBuilder modelBuilder);
    }
}