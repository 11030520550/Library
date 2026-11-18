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
    public class UsersController : ControllerBase
    {
        private readonly LIBRARYContext _context;

        // הזרקת ה-DbContext לקונטרולר דרך הבנאי
        public UsersController(LIBRARYContext context)
        {
            _context = context;
        }

        // דוגמה לפעולת Get לשליפת כל המשתמשים (כולל שדות חדשים)
        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            var usersResult = await _context.Users
                .Select(u => new {
                    u.UserID, // מפתח ראשי מעודכן
                    u.FullName, // השתנה מקומבינציית FirstName/LastName
                    u.Email,
                    u.Role, // עודכן מקומבינציית Role string
                    u.Status, // שדה חדש לאישור אדמין
                    u.SubscriptionType, // שדה חדש למנוי
                    u.SubscriptionEndDate, // שדה חדש למנוי
                    u.RegistrationDate,
                    u.IsAdmin // נשאר
                })
                .ToListAsync();

            return Ok(usersResult);
        }

        // דוגמה לפעולת Get לשליפת משתמש לפי מזהה (כולל שדות חדשים)
        [HttpGet("GetUserById/{userId}")]
        public async Task<ActionResult<object>> GetUser(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserID == userId) // שינוי ל-UserID
                .Select(u => new {
                    u.UserID,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.Status, // שדה חדש
                    u.SubscriptionType, // שדה חדש
                    u.SubscriptionEndDate, // שדה חדש
                    u.RegistrationDate,
                    u.IsAdmin
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // הוספת משתמש חדש (Register) - עודכן לעבוד עם FullName ו-UserID
        [HttpPost("AddUser")]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            // לוודא שהשדות מלאים (עודכן לבדיקת FullName)
            if (user == null || string.IsNullOrEmpty(user.FullName) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest("All fields are required.");
            }

            // ברירות המחדל (Pending, Member, None) מוגדרות ב-Model
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // CreatedAtAction עודכן ל-UserID
            return CreatedAtAction(nameof(GetUser), new { userId = user.UserID }, user);
        }

        // דוגמה לעדכון משתמש קיים (עודכן לעבוד עם UserID)
        [HttpPut("UpdateUserById/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, User user)
        {
            if (userId != user.UserID) // שינוי ל-UserID
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userId))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // דוגמה למחיקת משתמש (עודכן לעבוד עם UserID)
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int userId)
        {
            return _context.Users.Any(e => e.UserID == userId); // שינוי ל-UserID
        }
    }
}