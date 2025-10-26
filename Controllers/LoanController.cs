using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using BookLibraryApp.Data; // Assuming this is where LibraryDbContext lives
using BookLibraryApp.Models.Entities;

namespace BookLibraryApp.Controllers
{
    // The Loan features here are for ALL LOGGED-IN USERS (Patrons)
    [Authorize]
    public class LoanController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Inject the DbContext and UserManager
        public LoanController(LibraryDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Loan (Default Action)
        // Redirects the base route to the more specific MyLoans action
        public IActionResult Index()
        {
            return RedirectToAction("MyLoans");
        }

        // POST: Loan/Checkout/5
        // This action is called when a user clicks the 'Borrow' button on a Book's page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int id) // 'id' is the BookId
        {
            // 1. Get the current logged-in user's ID
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized("User ID could not be retrieved.");
            }

            // 2. Check if the book exists
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound($"Book with ID {id} was not found.");
            }

            // 3. Check for existing active loan for this book and user
            bool existingActiveLoan = await _context.Loans
                .AnyAsync(l => l.BookId == id && l.UserId == userId && l.IsActive);

            if (existingActiveLoan)
            {
                TempData["Warning"] = $"You already have active digital access to '{book.Title}'.";
                // 🔑 FIX 1: Redirect user to the reading view in the public CatalogController
                return RedirectToAction("Read", "Catalog", new { id = book.BookId });
            }

            // 4. Create the new digital loan record
            var newLoan = new Loan
            {
                BookId = id,
                UserId = userId,
                LoanDate = DateTime.Now,
                // Digital access lasts for 14 days
                DueDate = DateTime.Now.AddDays(14),
                IsActive = true
            };

            _context.Loans.Add(newLoan);
            await _context.SaveChangesAsync();

            // 5. Success: Redirect the user to the reading view
            TempData["Success"] = $"Successfully gained digital access to '{book.Title}' for 14 days. Enjoy!";

            // 🔑 FIX 2: Redirect the user to the reading view in the public CatalogController
            return RedirectToAction("Read", "Catalog", new { id = book.BookId });
        }

        // GET: Loan/MyLoans
        // Shows the user all their currently active digital loans
        public async Task<IActionResult> MyLoans()
        {
            var userId = _userManager.GetUserId(User);

            var activeLoans = await _context.Loans
                .Include(l => l.Book)
                .Where(l => l.UserId == userId && l.IsActive)
                .ToListAsync();

            return View(activeLoans);
        }

        // POST: Loan/EndAccess/5
        // Allows a user to manually end their digital access early (return the book).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndAccess(int id) // 'id' is the LoanId
        {
            var userId = _userManager.GetUserId(User);

            // Fetch the loan, including the book for a good success message
            var loan = await _context.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);

            // Safety check: Ensure loan exists, is active, and belongs to the current user
            if (loan == null || loan.UserId != userId || !loan.IsActive)
            {
                TempData["Error"] = "Cannot end access for an invalid or inactive loan.";
                return RedirectToAction("MyLoans");
            }

            // Set the loan to inactive
            loan.IsActive = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Digital access to '{loan.Book.Title}' has been ended.";
            return RedirectToAction("MyLoans");
        }
    }
}