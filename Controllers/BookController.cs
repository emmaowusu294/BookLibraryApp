using BookLibraryApp.Models.Entities;
using BookLibraryApp.Models.ViewModels;
using BookLibraryApp.Services; // Contains the IBookService interface
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// 🔑 NEW USING STATEMENTS for file handling
using Microsoft.AspNetCore.Hosting; // Needed for IWebHostEnvironment
using System.IO;                  // Needed for Path operations
using System;                     // Needed for Guid

namespace BookLibraryApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly LibraryDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment; // 🔑 Inject WebHostEnvironment

        public BookController(
            IBookService bookService,
            LibraryDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment webHostEnvironment) // 🔑 Add parameter
        {
            _bookService = bookService;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment; // 🔑 Assign
        }

        // --- Helper Method to Save Cover Image ---
        private async Task<string?> SaveCoverImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null; // No file uploaded or empty file
            }

            // 1. Define the folder path within wwwroot
            //    Example: wwwroot/images/covers/
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "covers");
            
            // 2. Ensure the directory exists, create if not
            Directory.CreateDirectory(uploadsFolder);

            // 3. Generate a unique file name to avoid overwriting existing files
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4. Save the file to the server's disk
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                // Log the error (you might want to inject ILogger here)
                Console.WriteLine($"Error saving file: {ex.Message}");
                return null; // Return null if saving fails
            }

            // 5. Return the *relative web path* to store in the database
            //    Example: /images/covers/guid_mybook.jpg
            return "/images/covers/" + uniqueFileName;
        }

        // --- Helper Method to Delete Cover Image ---
        private void DeleteCoverImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            // Convert web path to physical path
            string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));

            try
            {
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
            catch (Exception ex)
            {
                 // Log the error (you might want to inject ILogger here)
                 Console.WriteLine($"Error deleting file {physicalPath}: {ex.Message}");
            }
        }


        // GET: Book (Index) - Unchanged
        public async Task<IActionResult> Index(string searchString)
        {
            var books = await _bookService.GetAllBooksAsync(searchString);
            ViewData["CurrentFilter"] = searchString;
            return View(books);
        }

        // GET: Book/Create - Unchanged
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create - MODIFIED
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 🔑 Save the uploaded image first
                string imagePath = await SaveCoverImageAsync(model.CoverImageFile);
                model.CoverImageUrl = imagePath; // Set the path in the ViewModel

                // Now pass the model (with the path) to the service
                await _bookService.AddBookAsync(model);
                TempData["Success"] = $"Book '{model.Title}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Book/Edit/5 - Unchanged
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Edit/5 - MODIFIED
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookViewModel model)
        {
            // Check if the BookId is valid before proceeding
             if (model.BookId <= 0)
             {
                 return NotFound();
             }

            if (ModelState.IsValid)
            {
                // 🔑 Check if a NEW image was uploaded
                if (model.CoverImageFile != null)
                {
                    // 1. Get the existing book record to find the old image path
                    var existingBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.BookId == model.BookId);
                    if (existingBook != null && !string.IsNullOrEmpty(existingBook.CoverImageUrl))
                    {
                        // 2. Delete the OLD image file
                        DeleteCoverImage(existingBook.CoverImageUrl);
                    }

                    // 3. Save the NEW image file
                    string newImagePath = await SaveCoverImageAsync(model.CoverImageFile);
                    model.CoverImageUrl = newImagePath; // Update path in the ViewModel
                }
                // If no new file was uploaded, model.CoverImageUrl retains the hidden field value
                // and the BookService will use that existing path.

                bool success = await _bookService.UpdateBookAsync(model);
                if (!success)
                {
                    // Maybe the book was deleted by another user?
                    ModelState.AddModelError("", "Unable to save changes. The book might have been deleted.");
                    return View(model); // Return view with error
                }
                TempData["Success"] = $"Book '{model.Title}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Book/Delete/5 - Unchanged
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Delete/5 - MODIFIED (to also delete image)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1. Get the book details BEFORE deleting from DB to get the image path
            var bookToDelete = await _bookService.GetBookByIdAsync(id);
            if (bookToDelete == null)
            {
                return NotFound();
            }
            string imagePathToDelete = bookToDelete.CoverImageUrl; // Store path

            // 2. Delete the book record from the database
            bool success = await _bookService.DeleteBookAsync(id);

            if (!success)
            {
                TempData["Error"] = "Failed to delete the book record.";
                return RedirectToAction(nameof(Index)); // Or return error view
            }

            // 3. If DB delete was successful, delete the image file
            DeleteCoverImage(imagePathToDelete);

            TempData["Success"] = $"Book '{bookToDelete.Title}' deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}