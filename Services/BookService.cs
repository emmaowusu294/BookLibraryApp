using BookLibraryApp.Models.Entities;
using BookLibraryApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BookLibraryApp.Services
{
    // The class now implements the IBookService interface
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context;
        }

        // 🟢 Get all books with author names (Now Async)
        public async Task<IEnumerable<BookViewModel>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Select(b => new BookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author != null ? b.Author.Name : "Unknown"
                })
                .ToListAsync(); // Use ToListAsync()
        }

        // 🔵 Get a single book by ID (Now Async)
        public async Task<BookViewModel?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.BookId == id); // Use FirstOrDefaultAsync()

            if (book == null) return null;

            return new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorId = book.AuthorId,
                AuthorName = book.Author?.Name
            };
        }

        // 🟡 Add a new book (Now Async)
        public async Task AddBookAsync(BookViewModel model)
        {
            // ✅ Find or create author (Now Async)
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == model.AuthorName);
            if (author == null)
            {
                author = new Author { Name = model.AuthorName };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync(); // Save the new author first
            }

            var newBook = new Book
            {
                Title = model.Title,
                AuthorId = author.AuthorId
            };

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync(); // Use SaveChangesAsync()
        }

        // 🟠 Update existing book (Now Async)
        public async Task<bool> UpdateBookAsync(BookViewModel model)
        {
            var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.BookId == model.BookId);
            if (book == null) return false;

            // Find or create author (Now Async)
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == model.AuthorName);
            if (author == null)
            {
                author = new Author { Name = model.AuthorName };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync(); // Save the new author
            }

            book.Title = model.Title;
            book.AuthorId = author.AuthorId;

            await _context.SaveChangesAsync();
            return true;
        }

        // 🔴 Delete a book (Now Async)
        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id); // FindAsync() is implicitly async
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        // Retaining the synchronous methods for compatibility, though you should
        // use the async versions in the controller.
        public List<BookViewModel> GetAllBooks() => throw new NotImplementedException("Use GetAllBooksAsync()");
        public BookViewModel GetBookById(int id) => throw new NotImplementedException("Use GetBookByIdAsync()");
        public void AddBook(BookViewModel model) => throw new NotImplementedException("Use AddBookAsync()");
        public void UpdateBook(BookViewModel model) => throw new NotImplementedException("Use UpdateBookAsync()");
        public void DeleteBook(int id) => throw new NotImplementedException("Use DeleteBookAsync()");
    }
}