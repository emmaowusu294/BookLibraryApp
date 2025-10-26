using BookLibraryApp.Models.Entities;
using BookLibraryApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryApp.Services
{
    // this class implements the ILoanService interface
    public class LoanService : ILoanService
    {
        private readonly LibraryDbContext _context;

        // Standard loan period in days (e.g., 14 days)
        private const int LoanPeriodDays = 14;

        public LoanService(LibraryDbContext context)
        {
            _context = context;
        }

        // Helper method to map Loan Entity to Loan ViewModel
        private LoanViewModel MapToViewModel(Loan loan)
        {
            return new LoanViewModel
            {
                LoanId = loan.LoanId,
                BookId = loan.BookId,
                PatronId = loan.PatronId,
                CheckoutDate = loan.CheckoutDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,

                // Navigation data for display: Must be included via .Include()
                BookTitle = loan.Book.Title,
                PatronFullName = $"{loan.Patron.FirstName} {loan.Patron.LastName}"
            };
        }

        // ---------------------------------------------------------------------
        // 1. Get All Loans
        // ---------------------------------------------------------------------
        public async Task<IEnumerable<LoanViewModel>> GetAllLoansAsync()
        {
            // Eagerly load the related Book and Patron data using .Include()
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Patron)
                .ToListAsync();

            // Map the entities to ViewModels
            return loans.Select(MapToViewModel).ToList();
        }

        // ---------------------------------------------------------------------
        // 2. Get Loan by ID
        // ---------------------------------------------------------------------
        public async Task<LoanViewModel?> GetLoanByIdAsync(int id)
        {
            // Find the loan and eagerly load related data
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Patron)
                .FirstOrDefaultAsync(l => l.LoanId == id);

            if (loan == null)
            {
                return null;
            }

            return MapToViewModel(loan);
        }

        // ---------------------------------------------------------------------
        // 3. Checkout Book (Create new Loan record)
        // ---------------------------------------------------------------------
        /* public async Task CheckoutBookAsync(int bookId, int patronId)
         {
             // Optional: Add logic here to check if the book is already checked out!
             // E.g.: var existingLoan = await _context.Loans.AnyAsync(l => l.BookId == bookId && l.ReturnDate == null);

             var now = DateTime.Now;

             var newLoan = new Loan
             {
                 BookId = bookId,
                 PatronId = patronId,
                 CheckoutDate = now,
                 // Business Rule: DueDate is 14 days from checkout
                 DueDate = now.AddDays(LoanPeriodDays),
                 ReturnDate = null // Initially null
             };

             _context.Loans.Add(newLoan);
             await _context.SaveChangesAsync();
         }*/

        // 3. Checkout Book (Create new Loan record)
        public async Task<bool> CheckoutBookAsync(int bookId, int patronId)
        {
            // CRITICAL BUSINESS RULE: Prevent Duplicate Checkouts
            // Check if there is ANY existing loan for this book where ReturnDate is NULL.
            var isActiveLoan = await _context.Loans
                .AnyAsync(l => l.BookId == bookId && l.ReturnDate == null);

            if (isActiveLoan)
            {
                // Return false if the book is already checked out
                return false;
            }

            // If the book is available, proceed with checkout
            var now = DateTime.Now;

            var newLoan = new Loan
            {
                BookId = bookId,
                PatronId = patronId,
                CheckoutDate = now,
                DueDate = now.AddDays(LoanPeriodDays),
                ReturnDate = null
            };

            _context.Loans.Add(newLoan);
            await _context.SaveChangesAsync();

            // Return true for successful checkout
            return true;
        }

        // ---------------------------------------------------------------------
        // 4. Return Book (Update ReturnDate)
        // ---------------------------------------------------------------------
        public async Task<bool> ReturnBookAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return false;
            }

            // Only update if it hasn't already been returned
            if (loan.ReturnDate == null)
            {
                loan.ReturnDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            return true; // Already returned, consider it successful
        }

        // ---------------------------------------------------------------------
        // 5. Delete Loan
        // ---------------------------------------------------------------------
        public async Task<bool> DeleteLoanAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return false;
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return true;
        }


        // New method implementation for overdue report
        public async Task<IEnumerable<LoanViewModel>> GetOverdueLoansAsync()
        {
            var overdueLoans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Patron)
                // WHERE clause: ReturnDate is NULL AND DueDate is before today
                .Where(l => l.ReturnDate == null && l.DueDate.Date < DateTime.Now.Date)
                .ToListAsync();

            // Map the filtered entities to ViewModels
            return overdueLoans.Select(MapToViewModel).ToList();
        }
    }
}