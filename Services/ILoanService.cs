using BookLibraryApp.Models.Entities;

namespace BookLibraryApp.Services
{
    public interface ILoanService
    {
        // For Admin View: Fetch all loans with related Book and User data
        Task<IEnumerable<Loan>> GetAllLoansAsync();

        // For Admin View: Fetch all overdue/expired digital loans
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();

        // For Patron/Self-Service: Get all active loans for a specific User
        Task<IEnumerable<Loan>> GetActiveLoansByUserIdAsync(string userId);

        // Gets a single loan record
        Task<Loan?> GetLoanByIdAsync(int id);

        // 🔑 Simplified Checkout for Patron Self-Service
        // This is now an internal service method. The Controller handles the Identity check.
        Task<bool> DigitalCheckoutAsync(int bookId, string userId);

        // Return is now an "End Access" action for Admins to manually stop access
        Task<bool> EndAccessAsync(int loanId);

        // Admin-only: Deletes a loan record
        Task<bool> DeleteLoanAsync(int loanId);
    }
}