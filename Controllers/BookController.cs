using BookLibraryApp.Models;
using BookLibraryApp.Models.Entities;
using BookLibraryApp.Models.ViewModels;
using BookLibraryApp.Services;
using Microsoft.AspNetCore.Mvc;




namespace BookLibraryApp.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        // GET: Book
        public IActionResult Index()
        {
            var books = _bookService.GetAllBooks();
            return View(books);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                _bookService.AddBook(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Book/Edit/5
        public IActionResult Edit(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                _bookService.UpdateBook(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Book/Delete/5
        public IActionResult Delete(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _bookService.DeleteBook(id);
            return RedirectToAction(nameof(Index));
        }
    }




}