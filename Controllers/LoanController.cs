using BookLibraryApp.Services;
using BookLibraryApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookLibraryApp.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService; // Need for dropdowns
        private readonly IPatronService _patronService; // Need for dropdowns

        // Injecting all three required services
        public LoanController(
            ILoanService loanService, 
            IBookService bookService, 
            IPatronService patronService)
        {
            _loanService = loanService;
            _bookService = bookService;
            _patronService = patronService;
        }

        // Helper method to load Books and Patrons into ViewBag for dropdowns
        private async Task LoadDropdownsAsync()
        {
            // 1. Await the async service calls
            var books = await _bookService.GetAllBooksAsync();
            var patrons = await _patronService.GetAllPatrons(); // <-- MUST use the async method

            // 2. Use 'FullName' for the Patron SelectList
            ViewBag.Books = new SelectList(books, "BookId", "Title");
            ViewBag.Patrons = new SelectList(patrons, "PatronId", "FullName"); // <-- MUST use FullName
        }

        // ---------------------------------------------------------------------
        // INDEX (GET: /Loan) - List All Loans
        // ---------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var loans = await _loanService.GetAllLoansAsync();
            return View(loans);
        }

        // ---------------------------------------------------------------------
        // DETAILS (GET: /Loan/Details/5)
        // ---------------------------------------------------------------------
        public async Task<IActionResult> Details(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            return View(loan);
        }

        // ---------------------------------------------------------------------
        // CHECKOUT (GET: /Loan/Checkout) - Shows the form
        // ---------------------------------------------------------------------
        public async Task<IActionResult> Checkout()
        {
            await LoadDropdownsAsync();
            
            // Initialize the ViewModel with current date defaults
            var model = new LoanViewModel 
            {
                CheckoutDate = DateTime.Now.Date,
                DueDate = DateTime.Now.Date.AddDays(14)
            };
            
            return View(model);
        }

        // ---------------------------------------------------------------------
        // CHECKOUT (POST: /Loan/Checkout) - Handles the form submission
        // ---------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout([Bind("BookId,PatronId")] LoanViewModel model)
        {
            // Only validate the required fields for checkout
            if (ModelState.IsValid)
            {
                await _loanService.CheckoutBookAsync(model.BookId, model.PatronId);
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, reload dropdowns before returning the view
            await LoadDropdownsAsync();
            return View(model);
        }
        
        // ---------------------------------------------------------------------
        // RETURN (POST: /Loan/Return/5) - Handles returning a book
        // ---------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id)
        {
            bool success = await _loanService.ReturnBookAsync(id);

            if (!success)
            {
                // This might mean the loan ID was invalid or already returned
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE (GET: /Loan/Delete/5)
        // This action fetches the record and sends it to the Delete.cshtml view for confirmation.
        // ---------------------------------------------------------------------

        public async Task<IActionResult> Delete(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // ---------------------------------------------------------------------
        // DELETE (POST: /Loan/Delete/5)
        // ---------------------------------------------------------------------
        // Typically a POST to avoid accidental deletion via GET link
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool success = await _loanService.DeleteLoanAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}