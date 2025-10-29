using BookLibraryApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookLibraryApp.Models.Entities;
using BookLibraryApp.Models.ViewModels; // Contains BookViewModel and BookDetailsViewModel

namespace BookLibraryApp.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IBookService _bookService;
        private readonly LibraryDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CatalogController(
            IBookService bookService,
            LibraryDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _bookService = bookService;
            _context = context;
            _userManager = userManager;
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
            var bookViewModel = await _bookService.GetBookByIdAsync(id);
            if (bookViewModel == null)
            {
                return NotFound();
            }

            // 1. 🔑 FIX: Map ALL properties from BookViewModel to BookDetailsViewModel
            var model = new BookDetailsViewModel
            {
                BookId = bookViewModel.BookId,
                Title = bookViewModel.Title,
                AuthorName = bookViewModel.AuthorName,
                AuthorId = bookViewModel.AuthorId,

                // 🚨 CRITICAL FIX: Ensure new metadata is mapped 🚨
                Description = bookViewModel.Description,
                PublicationYear = bookViewModel.PublicationYear,
                Genre = bookViewModel.Genre,
                CoverImageUrl = bookViewModel.CoverImageUrl,

                HasActiveLoan = false // Default
            };

            // 2. Check loan status only if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);

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
            var userId = _userManager.GetUserId(User);

            // Fetch the book entity to get all metadata for display
            var book = await _context.Books
                .Include(b => b.Author) // Include author if needed for display
                .FirstOrDefaultAsync(b => b.BookId == id);

            // Security Check
            bool hasAccess = await _context.Loans
                .AnyAsync(l => l.BookId == id && l.UserId == userId && l.IsActive);

            if (!hasAccess)
            {
                TempData["Error"] = "Access denied. Please check out the book first.";
                return RedirectToAction("Details", new { id = id });
            }

            // Pass the required display data to ViewData
            if (book != null)
            {
                ViewData["Title"] = $"Reading: {book.Title}";
                ViewData["BookTitle"] = book.Title;
                ViewData["AuthorName"] = book.Author?.Name;
                ViewData["CoverImageUrl"] = book.CoverImageUrl;
                ViewData["BookId"] = book.BookId;
            }

            return View();
        }
    }
}