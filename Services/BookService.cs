using BookLibraryApp.Models.Entities;
using BookLibraryApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq; // Add this for .Select

namespace BookLibraryApp.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context;
        }

        // Helper method to map Book Entity -> BookViewModel (to avoid repetition)
        private BookViewModel MapBookToViewModel(Book book)
        {
            return new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorId = book.AuthorId,
                AuthorName = book.Author?.Name, // Use null-conditional operator

                // 🔑 MAP NEW FIELDS
                Description = book.Description,
                PublicationYear = book.PublicationYear,
                Genre = book.Genre,
                CoverImageUrl = book.CoverImageUrl
            };
        }


        // 🟢 Get all books with author names (Now with Search Filter)
        public async Task<IEnumerable<BookViewModel>> GetAllBooksAsync(string? searchString)
        {
            var query = _context.Books.Include(b => b.Author).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerSearch = searchString.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(lowerSearch) ||
                    (b.Author != null && b.Author.Name.ToLower().Contains(lowerSearch)));
            }

            // Apply sorting before projection for better DB performance potential
            query = query.OrderBy(b => b.Title);

            // 🔑 UPDATED SELECT: Now maps all new fields
            return await query
                .Select(b => new BookViewModel // Direct projection can be efficient too
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author != null ? b.Author.Name : "Unknown",
                    Description = b.Description,
                    PublicationYear = b.PublicationYear,
                    Genre = b.Genre,
                    CoverImageUrl = b.CoverImageUrl
                })
                .ToListAsync();
        }

        // 🔵 Get a single book by ID (Now Async)
        public async Task<BookViewModel?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null) return null;

            // 🔑 Use the helper or ensure all fields are mapped here
            return MapBookToViewModel(book);
        }

        // 🟡 Add a new book (Now Async)
        public async Task AddBookAsync(BookViewModel model)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == model.AuthorName);
            if (author == null)
            {
                author = new Author { Name = model.AuthorName ?? "Unknown Author" }; // Provide default if null
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
            }

            var newBook = new Book
            {
                Title = model.Title,
                AuthorId = author.AuthorId,

                // 🔑 MAP NEW FIELDS from ViewModel to Entity
                Description = model.Description,
                PublicationYear = model.PublicationYear,
                Genre = model.Genre,
                CoverImageUrl = model.CoverImageUrl // This should contain the path saved by the controller
            };

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();
        }

        // 🟠 Update existing book (Now Async)
        public async Task<bool> UpdateBookAsync(BookViewModel model)
        {
            var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.BookId == model.BookId);
            if (book == null) return false;

            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == model.AuthorName);
            if (author == null)
            {
                author = new Author { Name = model.AuthorName ?? "Unknown Author" };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
            }

            // Update existing properties
            book.Title = model.Title;
            book.AuthorId = author.AuthorId;

            // 🔑 UPDATE NEW FIELDS from ViewModel to Entity
            book.Description = model.Description;
            book.PublicationYear = model.PublicationYear;
            book.Genre = model.Genre;
            // Only update image URL if a new one was provided (handled in Controller, but good practice)
            if (!string.IsNullOrEmpty(model.CoverImageUrl))
            {
                book.CoverImageUrl = model.CoverImageUrl;
            }


            await _context.SaveChangesAsync();
            return true;
        }

        // 🔴 Delete a book (Now Async) - No changes needed here for metadata
        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            // TODO: Consider deleting the associated image file from wwwroot here

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get Available Books (Digital Library Logic)
        public async Task<IEnumerable<BookViewModel>> GetAvailableBooksAsync()
        {
            // Assuming for a digital library, all books are generally available
            // unless specific logic for *current user* is needed.
            // This method might just return all books.

            var query = _context.Books.Include(b => b.Author).OrderBy(b => b.Title);

            // 🔑 UPDATED SELECT: Map all fields
            return await query
                .Select(b => MapBookToViewModel(b)) // Use helper for consistency
                .ToListAsync();
        }

        // Synchronous stubs remain unchanged
        public List<BookViewModel> GetAllBooks() => throw new NotImplementedException("Use GetAllBooksAsync()");
        public BookViewModel GetBookById(int id) => throw new NotImplementedException("Use GetBookByIdAsync()");
        public void AddBook(BookViewModel model) => throw new NotImplementedException("Use AddBookAsync()");
        public void UpdateBook(BookViewModel model) => throw new NotImplementedException("Use UpdateBookAsync()");
        public void DeleteBook(int id) => throw new NotImplementedException("Use DeleteBookAsync()");
    }
}