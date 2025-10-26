
using BookLibraryApp.Models.Entities;
using BookLibraryApp.Models.ViewModels;
using BookLibraryApp.Services; // Contains the IBookService interface
using Microsoft.AspNetCore.Authorization;
// 🔑 NEW USING STATEMENTS
using Microsoft.AspNetCore.Identity; // Needed for UserManager
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Needed for .AnyAsync() and DbContext access
using System.Threading.Tasks;


namespace BookLibraryApp.Controllers
{
    // The main actions for Index, Create, Edit, Delete are restricted to Admin
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        // 1. Dependency Injection: Use the INTERFACE (IBookService)
        private readonly IBookService _bookService;

        // 🔑 REQUIRED FIX: Define private fields for the new dependencies
        private readonly LibraryDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // 🔑 REQUIRED FIX: Update the constructor to accept the new dependencies
        public BookController(
            IBookService bookService,
            LibraryDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _bookService = bookService;
            _context = context; // Initialize the injected DbContext
            _userManager = userManager; // Initialize the injected UserManager
        }

        // ---------------------------------------------------------------------
        // GET: Book (Index)
        // ---------------------------------------------------------------------
        public async Task<IActionResult> Index(string searchString)
        {
            var books = await _bookService.GetAllBooksAsync(searchString);
            ViewData["CurrentFilter"] = searchString;
            return View(books);
        }

        // ---------------------------------------------------------------------
        // GET: Book/Create (and POST: Book/Create)
        // ---------------------------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _bookService.AddBookAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }



        // ---------------------------------------------------------------------
        // GET: Book/Edit/5 (and POST: Book/Edit/5)
        // ---------------------------------------------------------------------
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = await _bookService.UpdateBookAsync(model);
                if (!success)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // ---------------------------------------------------------------------
        // GET: Book/Delete/5 (and POST: Book/Delete/5)
        // ---------------------------------------------------------------------
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool success = await _bookService.DeleteBookAsync(id);

            if (!success)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }


        
    }
}