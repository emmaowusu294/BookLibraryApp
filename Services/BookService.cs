using BookLibraryApp.Models.Entities;
using BookLibraryApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryApp.Services
{
    public class BookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context;
        }

        // 🟢 Get all books with author names
        public List<BookViewModel> GetAllBooks()
        {
            return _context.Books
                .Include(b => b.Author)
                .Select(b => new BookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author != null ? b.Author.Name : "Unknown"
                })
                .ToList();
        }

        // 🔵 Get a single book by ID
        public BookViewModel GetBookById(int id)
        {
            var book = _context.Books
                .Include(b => b.Author)
                .FirstOrDefault(b => b.BookId == id);

            if (book == null) return null;

            return new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorId = book.AuthorId,
                AuthorName = book.Author?.Name
            };
        }

        // 🟡 Add a new book
        public void AddBook(BookViewModel model)
        {
            // ✅ Find or create author
            var author = _context.Authors.FirstOrDefault(a => a.Name == model.AuthorName);
            if (author == null)
            {
                author = new Author { Name = model.AuthorName };
                _context.Authors.Add(author);
                _context.SaveChanges();
            }

            var newBook = new Book
            {
                Title = model.Title,
                AuthorId = author.AuthorId
            };

            _context.Books.Add(newBook);
            _context.SaveChanges();
        }

        // 🟠 Update existing book
        public void UpdateBook(BookViewModel model)
        {
            var book = _context.Books.Include(b => b.Author).FirstOrDefault(b => b.BookId == model.BookId);
            if (book == null) return;

            var author = _context.Authors.FirstOrDefault(a => a.Name == model.AuthorName);
            if (author == null)
            {
                author = new Author { Name = model.AuthorName };
                _context.Authors.Add(author);
                _context.SaveChanges();
            }

            book.Title = model.Title;
            book.AuthorId = author.AuthorId;

            _context.SaveChanges();
        }

        // 🔴 Delete a book
        public void DeleteBook(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return;

            _context.Books.Remove(book);
            _context.SaveChanges();
        }
    }
}
