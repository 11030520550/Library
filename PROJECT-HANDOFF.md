# PROJECT-HANDOFF — מערכת ספרייה (LibraryWithSql_PO)

עודכן: 2026-11-19

## 1. מה זה

\u200FWeb API לניהול ספרייה: ספרים, משתמשים, קטגוריות והשאלות.
הצד־לקוח (Angular, `http://localhost:4200` לפי הגדרת CORS) **אינו** נמצא ב־Repository הזה.

| רכיב | גרסה |
|---|---|
| \u200F.NET / ASP.NET Core Web API | \u200F`net6.0` (נבנה עם SDK 8.0.303) |
| \u200FEntity Framework Core (SqlServer + Tools) | \u200F6.0.0 |
| \u200FSwagger (Swashbuckle) | \u200F6.5.0 |
| מסד נתונים | \u200FSQL Server Express, מסד `LIBRARY`, \u200FCollation \u200F`Hebrew_CI_AS` |

## 2. מבנה התיקיות

```
LibraryWithSql_PO/
├── Controllers/
│   ├── BooksController.cs           # api/Books
│   ├── UsersController.cs           # api/Users
│   └── WeatherForecastController.cs # שארית מתבנית הפרויקט
├── models/
│   ├── LIBRARYContext.cs            # DbContext + מיפוי הטבלאות
│   ├── Book.cs  User.cs  Loan.cs  Category.cs
├── Properties/launchSettings.json   # פורטים ופרופילי הרצה
├── Program.cs                       # DI, CORS, Swagger
├── appsettings.json                 # Connection String
├── 1.docx                           # מסמך תכנון שלבי הפיתוח
└── LibraryWithSql_PO.sln / .csproj
```

## 3. פתיחה במחשב אחר

```cmd
git -c http.sslBackend=schannel clone https://github.com/11030520550/Library.git LibraryWithSql_PO
cd LibraryWithSql_PO
dotnet restore
dotnet build
dotnet run --launch-profile LibraryWithSql_PO
```

- \u200F`-c http.sslBackend=schannel` נדרש רק במחשב עם NetFree. אותו דבר ל־`fetch`, \u200F`pull` ו־`push`.
- נדרש \u200F.NET SDK 6 ומעלה. ה־SDK 8 עובד, אבל צריך גם את **ASP.NET Core Runtime 6** כדי להריץ (`dotnet --list-runtimes`).
- כתובות: \u200F`https://localhost:7248` ו־`http://localhost:5221`. \u200FHTTP מופנה אוטומטית ל־HTTPS.
- \u200FSwagger: \u200F`https://localhost:7248/swagger`
- אם הדפדפן מתריע על התעודה: `dotnet dev-certs https --trust`
- ב־Visual Studio: פותחים את `LibraryWithSql_PO.sln` ומריצים את הפרופיל `LibraryWithSql_PO`.

## 4. \u200FSQL Server והגדרות חיבור

ב־`appsettings.json`:

```
Server=DESKTOP-8ED3CL9\SQLEXPRESS;Database=LIBRARY;Trusted_Connection=True;
```

- ההזדהות היא Windows Authentication, כלומר אין סיסמה בקובץ.
- `DESKTOP-8ED3CL9` הוא שם המחשב המקורי. במחשב אחר צריך להחליף אותו בשם השרת המקומי, למשל `.\SQLEXPRESS` או `(localdb)\MSSQLLocalDB`.
- ⚠️ **מסד הנתונים לא נמצא ב־GitHub.** הוא קיים רק במחשב המקורי, ואין בפרויקט Migrations או סקריפט יצירה. כדי לעבוד במחשב אחר עם אותו מידע צריך גיבוי של המסד (\u200E`.bak`) או סקריפט Schema ונתונים.
- מצב המסד במחשב המקורי, נכון ל־2026-11-19:

| טבלה | שורות | עמודות עיקריות |
|---|---|---|
| \u200F`Books` | 28 | \u200FBookID, \u200FTitle, \u200FAuthor, \u200FPublishedYear, **Category (טקסט)**, \u200FSummary, \u200FISBN, \u200FAvailabilityStatus, \u200FAddedDate, \u200FPreviewContent, \u200FTotalCopies, \u200FAvailableCopies, \u200FCoverImageUrl |
| \u200F`Users` | 45 | \u200FUserId, **FirstName, LastName**, \u200FEmail, \u200FPasswordHash, \u200FRegistrationDate, \u200FIsAdmin, \u200FStatus, \u200FSubscriptionType, \u200FSubscriptionEndDate |
| \u200F`Loans` | 0 | \u200FLoanID, \u200FUserID ← Users, \u200FBookID ← Books, \u200FLoanDate, \u200FDueDate, \u200FReturnDate, \u200FStatus |
| \u200F`Categories` | — | **לא קיימת** |

## 5. מה עובד ומה לא (נבדק ב־2026-11-19)

| בדיקה | תוצאה |
|---|---|
| \u200F`dotnet restore` / `dotnet build` | ✅ \u200F0 שגיאות, 13 אזהרות |
| השרת עולה (`dotnet run`) | ✅ |
| \u200FSwagger UI ו־`swagger.json` | ✅ |
| חיבור ל־SQL Server | ✅ השרת זמין והשאילתות מגיעות למסד |
| \u200F`GET /WeatherForecast` | ✅ \u200F200 (שארית מהתבנית) |
| \u200F`GET /api/Books/GetBooks` | ❌ \u200F400 בלי `search` (הפרמטר הופך לחובה), ו־500 עם `search`: `Invalid object name 'Categories'` |
| \u200F`GET /api/Books/{id}` | ❌ \u200F500: `Invalid object name 'Categories'` |
| \u200F`GET /api/Books/SearchBooksByTitle/{title}` | ❌ \u200F500: `Invalid object name 'Categories'` |
| \u200F`POST /api/Books` | ❌ \u200F500: התנגשות JSON בין `BookID` ל־`BookId` במודל |
| \u200F`GET /api/Users/GetUsers` | ❌ \u200F500: `Invalid column name 'FullName'`, \u200F`'Role'` |
| \u200F`GET /api/Users/GetUserById/{id}` | ❌ \u200F500: אותה שגיאה |
| \u200F`POST /api/Users/AddUser` | ⚠️ \u200F400 על גוף ריק. דורש גם `Loans`. לא נבדק עם נתונים אמיתיים |
| \u200F`PUT` / `DELETE` (ספרים ומשתמשים) | לא נבדקו, כדי לא לשנות נתונים |

## 6. בעיות פתוחות

1. **אי־התאמה בין הקוד למסד הנתונים.** זו הבעיה המרכזית, והיא חוסמת את כל ה־API:
   - הקוד מצפה לטבלת `Categories` ולעמודה `Books.CategoryID`. במסד יש עמודת טקסט `Books.Category`, ואין טבלת קטגוריות.
   - הקוד מצפה ל־`Users.FullName` ול־`Users.Role`. במסד יש `FirstName` ו־`LastName`, ואין `Role`.
   - צריך להחליט: לעדכן את המסד לפי הקוד (Migration), או להתאים את הקוד למסד הקיים.
2. **`Book.BookId`.** בנוסף ל־`BookID` קיים מאפיין כפול, `public int BookId { get; internal set; }`. הוא שובר JSON ב־POST ו־PUT של ספרים.
3. **פרמטרים אופציונליים.** ב־`GetBooks` הפרמטר `search` הוא `string` רגיל. בגלל `Nullable=enable` הוא נחשב חובה (400). כדי שיהיה אופציונלי צריך `string?`.
4. **שדות ניווט שנחשבים חובה.** \u200F`Category`, \u200F`Loans`, \u200F`User` ו־`Book` מוגדרים כ־Non-nullable, ולכן ה־Model Validation דורש אותם ב־POST ו־PUT.
5. **Placeholders.** שמות ה־Foreign Keys ב־`LIBRARYContext` הם תבניות (`"FK__Books__CategoryI__..."`), ולא השמות האמיתיים במסד.
6. **אבטחה:**
   - אין Authentication או Authorization.
   - \u200F`AddUser` מקבל `PasswordHash` כמו שהוא, בלי Hashing.
   - התשובה של `AddUser` מחזירה את אובייקט המשתמש כולו, כולל `PasswordHash`.
7. **אזהרות Build:**
   - 12 × \u200FCS8618 (Nullable).
   - 1 × \u200FCS1030: `#warning` על Connection String ב־`OnConfiguring`.
8. **שאריות מהתבנית:** \u200F`WeatherForecast` ו־`WeatherForecastController`.
9. **גרסה לא נתמכת.** \u200F.NET 6 ו־EF Core 6 כבר לא נתמכים (סוף התמיכה: נובמבר 2024).
10. **ממשק חסר.** אין Controller ל־Loans או ל־Categories.
11. **הרשאות בתיקייה.** במחשב המקורי תיקיית הפרויקט בבעלות `SYSTEM`. מחיקה או שינוי שם של קבצים קיימים עלולים להיכשל בלי הרשאות מנהל.

## 7. המלצות להמשך (לפי סדר)

1. **החלטה על Schema.** לקבוע אם המסד הוא מקור האמת או הקוד, ולהתאים את השני.
2. **גיבוי המסד.** לגבות את `LIBRARY` (\u200F`.bak` או סקריפט) ולשמור אותו במקום נגיש למחשב השני.
3. **תיקון מינימלי לקריאה.** לתקן את חוסר ההתאמה, את `BookId` הכפול ואת `search` כך שה־GET-ים יעבדו.
4. **Controllers חסרים.** להוסיף Controllers ל־Categories ו־Loans (השאלה והחזרה, כולל עדכון `AvailableCopies`).
5. **אבטחה.** להוסיף Hashing לסיסמאות, התחברות עם JWT, הרשאות Admin/Member ו־DTOs, ולא להחזיר `PasswordHash`.
6. **ניקיון.** להסיר את קוד ה־WeatherForecast ולטפל באזהרות ה־Nullable.
7. **שדרוג.** לשקול מעבר ל־.NET 8 ול־EF Core 8.

## 8. Git

- הענף הפעיל הוא `main`.
- ענף הגיבוי `backup/main-before-replace-20261119` מכיל את התוכן הישן של ה־Repository.
- \u200F`.gitignore` מחריג את `.vs/`, \u200F`bin/`, \u200F`obj/`, \u200F`*.user` וקבצים זמניים.
