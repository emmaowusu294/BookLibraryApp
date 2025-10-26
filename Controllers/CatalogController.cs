using BookLibraryApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// 🔑 NEW USING STATEMENTS
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Needed for UserManager
using Microsoft.EntityFrameworkCore; // Needed for .AnyAsync() and DbContext access
using BookLibraryApp.Models.Entities; // Assuming Loan entity is here


using BookLibraryApp.Models.ViewModels; // IMPORTANT: For the new ViewModel


namespace BookLibraryApp.Controllers
{
    // NO AUTHORIZE attribute here, allowing public access to Index and Details
    public class CatalogController : Controller
    {
        private readonly IBookService _bookService;

        // 🔑 REQUIRED: Define private fields for the new dependencies
        private readonly LibraryDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // 🔑 REQUIRED: Update the constructor to accept all dependencies
        public CatalogController(
            IBookService bookService,
            LibraryDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _bookService = bookService;
            _context = context;         // Initialize DbContext
            _userManager = userManager; // Initialize UserManager
        }

        // GET: /Catalog or /Catalog/Index (Public Book List with Search)
        public async Task<IActionResult> Index(string searchString)
        {
            var books = await _bookService.GetAllBooksAsync(searchString);

            ViewData["CurrentFilter"] = searchString;
            return View(books);
        }

        // GET: /Catalog/Details/5 (Public view for book details)
        public async Task<IActionResult> Details(int id)
        {
            // Use the service to get the base data
            var bookViewModel = await _bookService.GetBookByIdAsync(id);
            if (bookViewModel == null)
            {
                return NotFound();
            }

            // 1. Map the base ViewModel to the enhanced Details ViewModel
            var model = new BookDetailsViewModel
            {
                BookId = bookViewModel.BookId,
                Title = bookViewModel.Title,
                AuthorName = bookViewModel.AuthorName,
                AuthorId = bookViewModel.AuthorId, // Include existing properties
                HasActiveLoan = false // Default to false
            };

            // 2. Check loan status only if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);

                // Perform the async loan check against the database
                model.HasActiveLoan = await _context.Loans
                    .AnyAsync(l => l.BookId == id && l.UserId == userId && l.IsActive);
            }

            return View(model); // Pass the enhanced model to the view
        }

        // ---------------------------------------------------------------------
        // GET: Catalog/Read/5 - Digital Access Feature (Open to all logged-in users)
        // ---------------------------------------------------------------------
        [Authorize] // Only available to any logged-in user
        public async Task<IActionResult> Read(int id) // id is BookId
        {
            // 🔑 These lines now work because _userManager and _context are initialized
            var userId = _userManager.GetUserId(User);

            // Security Check: Does the user have an active loan for this book?
            bool hasAccess = await _context.Loans
                .AnyAsync(l => l.BookId == id && l.UserId == userId && l.IsActive);

            if (!hasAccess)
            {
                TempData["Error"] = "Access denied. Please check out the book first.";
                return RedirectToAction("Details", new { id = id });
            }

            // 🔑 NEW: Fetch the book and pass the title to the view
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                ViewData["Title"] = $"Reading: {book.Title}";
                ViewData["BookTitle"] = book.Title;
                ViewData["BookId"] = book.BookId;
            }
            // Logic to render the digital content (e.g., PDF link, chapter text)
            // ...

            return View();
        }
    }
}