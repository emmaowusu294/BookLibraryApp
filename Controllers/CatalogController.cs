using BookLibraryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryApp.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IBookService _bookService;

        public CatalogController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: /Catalog or /Catalog/Index (Public Book List with Search)
        // Note: This uses the existing book service method that includes search/filter logic.
        public async Task<IActionResult> Index(string searchString)
        {
            var books = await _bookService.GetAllBooksAsync(searchString);

            ViewData["CurrentFilter"] = searchString;
            return View(books);
        }

        // GET: /Catalog/Details/5 (Public view for book details)
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }
    }
}