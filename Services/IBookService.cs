using BookLibraryApp.Models.ViewModels;

namespace BookLibraryApp.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookViewModel>> GetAllBooksAsync(); // Async version
        Task<BookViewModel?> GetBookByIdAsync(int id);      // Async version
        Task AddBookAsync(BookViewModel model);             // Async version
        Task<bool> UpdateBookAsync(BookViewModel model);    // Async version
        Task<bool> DeleteBookAsync(int id);                 // Async version

        // Also add a synchronous version for use in non-async contexts if needed,
        // but it's best to keep everything async when hitting the DB.
        // We will keep the focus on the async methods.
    }
}