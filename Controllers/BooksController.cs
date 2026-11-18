using LibraryWithSql_PO.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWithSql_PO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LIBRARYContext _context;

        // הזרקת ה-DbContext לקונטרולר דרך הבנאי
        public BooksController(LIBRARYContext context)
        {
            _context = context;
        }

        // דוגמה לפעולת Get לשליפת כל הספרים (כולל שם הקטגוריה)
        [HttpGet]
        [Route("GetBooks")]
        public async Task<ActionResult<IEnumerable<object>>> GetBooks(string search = "", int? categoryId = null, bool onlyAvailable = false)
        {
            var booksQuery = _context.Books
                .Include(b => b.Category) // שולף גם את פרטי הקטגוריה (Many-to-One)
                .AsQueryable();

            // סינון לפי חיפוש חופשי (Title או Author)
            if (!string.IsNullOrEmpty(search))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
            }

            // סינון לפי קטגוריה (עכשיו עם ID)
            if (categoryId.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.CategoryID == categoryId.Value);
            }

            // סינון לפי זמינות (עכשיו עם מלאי)
            if (onlyAvailable)
            {
                booksQuery = booksQuery.Where(b => b.AvailableCopies > 0);
            }

            // בחירת שדות ספציפיים להחזרה (שדה מחושב לשם קטגוריה וזמינות)
            var booksResult = await booksQuery
                .Select(b => new {
                    b.BookID, // מפתח ראשי מעודכן
                    b.Title,
                    b.Author,
                    b.PublishedYear,
                    CategoryName = b.Category != null ? b.Category.CategoryName : "אחר", // השתנה מקטגוריה (string)
                    b.Summary,
                    b.ISBN,
                    b.PreviewContent, // שדה חדש לפרק התרשמות
                    b.TotalCopies, // שדה חדש למלאי
                    b.AvailableCopies, // שדה חדש למלאי זמין
                    b.CoverImageUrl, // שדה חדש לתמונה
                    IsAvailable = b.AvailableCopies > 0, // שדה מחושב
                    b.AddedDate
                })
                .ToListAsync();

            return Ok(booksResult);
        }

        // שליפת ספר על פי שם (כולל קטגוריה)
        [HttpGet("SearchBooksByTitle/{title}")]
        public async Task<ActionResult<IEnumerable<object>>> SearchBooksByTitle(string title)
        {
            var books = await _context.Books
                .Include(b => b.Category)
                .Where(b => b.Title.Contains(title))
                .Select(b => new { // בחירת שדות (כמו ב-GetBooks)
                    b.BookID,
                    b.Title,
                    b.Author,
                    b.PublishedYear,
                    CategoryName = b.Category != null ? b.Category.CategoryName : "אחר",
                    b.AvailableCopies,
                    b.CoverImageUrl
                })
                .ToListAsync();

            if (books == null || books.Count == 0)
            {
                return NotFound("No books found with the given title.");
            }

            return Ok(books);
        }

        // דוגמה לפעולת Get לשליפת ספר לפי מזהה (כולל קטגוריה ומלאי)
        [HttpGet("{bookId}")]
        public async Task<ActionResult<object>> GetBook(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.Category)
                .Where(b => b.BookID == bookId) // שינוי ל-BookID
                .Select(b => new {
                    b.BookID,
                    b.Title,
                    b.Author,
                    b.PublishedYear,
                    CategoryID = b.CategoryID, // החזרת ה-ID
                    CategoryName = b.Category != null ? b.Category.CategoryName : "אחר", // והשם
                    b.Summary,
                    b.ISBN,
                    b.PreviewContent, // שדה חדש
                    b.TotalCopies, // שדה חדש
                    b.AvailableCopies, // שדה חדש
                    b.CoverImageUrl, // שדה חדש
                    b.AddedDate
                })
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // דוגמה להוספת ספר חדש (עודכן לעבוד עם BookID ו-CategoryID)
        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(Book book)
        {
            // ודאי שהקטגוריה קיימת ב-Database לפני הקישור (CategoryID)
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // CreatedAtAction עודכן ל-BookID
            return CreatedAtAction(nameof(GetBook), new { bookId = book.BookID }, book);
        }

        // דוגמה לעדכון ספר קיים (עודכן לעבוד עם BookID)
        [HttpPut("{bookId}")]
        public async Task<IActionResult> UpdateBook(int bookId, Book book)
        {
            if (bookId != book.BookID) // שינוי ל-BookID
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(bookId))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // דוגמה למחיקת ספר (עודכן לעבוד עם BookID)
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int bookId)
        {
            return _context.Books.Any(e => e.BookID == bookId); // שינוי ל-BookID
        }
    }
}