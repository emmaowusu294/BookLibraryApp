using Microsoft.EntityFrameworkCore;
//using BookLibraryApp.Data; // Your DbContext namespace
using BookLibraryApp.Models.Entities; // Your Models namespace

namespace BookLibraryApp.Services
{
    public class LoanService : ILoanService
    {
        private readonly LibraryDbContext _context;

        public LoanService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            // Includes the Book and User (Patron) details for display in the Admin view.
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User) // Link to IdentityUser
                .ToListAsync();
        }

        // Admin Functionality
        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            // For digital access, "Overdue" means IsActive is true BUT DueDate is in the past.
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.IsActive && l.DueDate < DateTime.Now)
                .ToListAsync();
        }

        // Patron Functionality
        public async Task<IEnumerable<Loan>> GetActiveLoansByUserIdAsync(string userId)
        {
            // Loans that are currently active for the Patron
            return await _context.Loans
                .Include(l => l.Book)
                .Where(l => l.UserId == userId && l.IsActive && l.DueDate >= DateTime.Now)
                .ToListAsync();
        }

        public async Task<Loan?> GetLoanByIdAsync(int id)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        // 🔑 Implement the Checkout Logic here if you want to use the Service Pattern
        public async Task<bool> DigitalCheckoutAsync(int bookId, string userId)
        {
            // 1. Check for existing active loan
            bool existingLoan = await _context.Loans
                .AnyAsync(l => l.BookId == bookId && l.UserId == userId && l.IsActive);

            if (existingLoan) return false; // Already checked out

            // 2. Create the new digital loan record
            var newLoan = new Loan
            {
                BookId = bookId,
                UserId = userId,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                IsActive = true
            };

            _context.Loans.Add(newLoan);
            await _context.SaveChangesAsync();
            return true;
        }

        // Admin Action: Manually End Access for a user
        public async Task<bool> EndAccessAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null || !loan.IsActive) return false;

            loan.IsActive = false;
            // Optionally, log the date access was terminated
            // loan.ReturnDate = DateTime.Now; 

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLoanAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return false;

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}