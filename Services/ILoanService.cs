using BookLibraryApp.Models.ViewModels;

namespace BookLibraryApp.Services
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanViewModel>> GetAllLoansAsync();
        Task<LoanViewModel?> GetLoanByIdAsync(int id);

        // This method needs to pull the DueDate from the service logic
        Task CheckoutBookAsync(int bookId, int patronId);

        // This method updates the ReturnDate
        Task<bool> ReturnBookAsync(int id);

        Task<bool> DeleteLoanAsync(int id);
    }
}