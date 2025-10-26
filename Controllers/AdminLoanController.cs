
using BookLibraryApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibraryApp.Controllers
{
    // Restrict all actions in this controller to Admin role only
    [Authorize(Roles = "Admin")]
    public class AdminLoanController : Controller
    {
        private readonly LibraryDbContext _context;

        public AdminLoanController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: /AdminLoan - Shows all ACTIVE loans for all users
        public async Task<IActionResult> Index()
        {
            // Fetch all active loans, including the book details
            var activeLoans = await _context.Loans
                .Include(l => l.Book)
                .Where(l => l.IsActive)
                // Note: You may need a simple ViewModel here to fetch and display the 
                // Identity User's Email/Username alongside the Loan data.
                .ToListAsync();

            // Create Views/AdminLoan/Index.cshtml to display this list
            return View(activeLoans);
        }

        // GET: /AdminLoan/History - Shows ALL loans (active and inactive) for auditing
        public async Task<IActionResult> History()
        {
            var allLoans = await _context.Loans
                .Include(l => l.Book)
                .ToListAsync();

            // Create Views/AdminLoan/History.cshtml to display this full list
            return View(allLoans);
        }

        // POST: /AdminLoan/ForceEnd/5
        // Allows an admin to manually set any loan's IsActive = False
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceEnd(int id) // 'id' is the LoanId
        {
            var loan = await _context.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                TempData["Error"] = "Loan not found.";
                return RedirectToAction("Index");
            }

            loan.IsActive = false;
            await _context.SaveChangesAsync();

            // You'll want to display the actual username/email here, 
            // but UserId is used for now.
            TempData["Success"] = $"Access to '{loan.Book.Title}' for user {loan.UserId} has been forcefully ended.";
            return RedirectToAction("Index");
        }
    }
}