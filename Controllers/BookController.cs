using BookLibraryApp.Models.ViewModels;
using BookLibraryApp.Services; // Contains the IBookService interface
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks; // Required for async/await

namespace BookLibraryApp.Controllers
{
    public class BookController : Controller
    {
        // 1. Dependency Injection: Use the INTERFACE (IBookService)
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // ---------------------------------------------------------------------
        // GET: Book (Index)
        // ---------------------------------------------------------------------
        // Now 'async' and returns 'Task<IActionResult>'
        // GET: Book (Index)
        // Add the searchString parameter
        public async Task<IActionResult> Index(string searchString)
        {
            // Pass the search string to the service method
            var books = await _bookService.GetAllBooksAsync(searchString);

            // This keeps the search term in the search box after filtering
            ViewData["CurrentFilter"] = searchString;

            return View(books);
        }

        // ---------------------------------------------------------------------
        // GET: Book/Create
        // ---------------------------------------------------------------------
        // No DB calls, so it can remain synchronous (returns 'IActionResult')
        public IActionResult Create()
        {
            return View();
        }

        // ---------------------------------------------------------------------
        // POST: Book/Create
        // ---------------------------------------------------------------------
        // Now 'async' and returns 'Task<IActionResult>'
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Use 'await'
                await _bookService.AddBookAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // ---------------------------------------------------------------------
        // GET: Book/Edit/5
        // ---------------------------------------------------------------------
        // Now 'async' and returns 'Task<IActionResult>'
        public async Task<IActionResult> Edit(int id)
        {
            // Use 'await'
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // ---------------------------------------------------------------------
        // POST: Book/Edit/5
        // ---------------------------------------------------------------------
        // Now 'async' and returns 'Task<IActionResult>'
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Use 'await'
                bool success = await _bookService.UpdateBookAsync(model);
                if (!success) // Check if the update failed (e.g., book not found)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // ---------------------------------------------------------------------
        // GET: Book/Delete/5
        // ---------------------------------------------------------------------
        // Now 'async' and returns 'Task<IActionResult>'
        public async Task<IActionResult> Delete(int id)
        {
            // Use 'await'
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // ---------------------------------------------------------------------
        // POST: Book/Delete/5
        // ---------------------------------------------------------------------
        // Now 'async' and returns 'Task<IActionResult>'
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Use 'await'
            bool success = await _bookService.DeleteBookAsync(id);

            // It's good practice to check if the delete was successful
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}