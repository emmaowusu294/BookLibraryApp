using BookLibraryApp.Services;
using BookLibraryApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace BookLibraryApp.Controllers
{
    [Authorize(Roles = "Admin")]
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

        // Helper method to load Patrons and *Available* Books into ViewBag
        private async Task LoadDropdownsAsync()
        {
            //FIX REQUIRED HERE: Use the new method to get only available books
            var books = await _bookService.GetAvailableBooksAsync();

            var patrons = await _patronService.GetAllPatrons();

            // Creating SelectList items for the View
            ViewBag.Books = new SelectList(books, "BookId", "Title");
            ViewBag.Patrons = new SelectList(patrons, "PatronId", "FullName");
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
    // Reload dropdowns in case we need to return the view with errors
    await LoadDropdownsAsync();

    if (ModelState.IsValid)
    {
        // Await the service call, which now returns true/false
        bool success = await _loanService.CheckoutBookAsync(model.BookId, model.PatronId);

        if (success)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            //  If success is false, add a model error
            ModelState.AddModelError(string.Empty, "This book is currently checked out and unavailable for loan.");
            
            // Re-select the book/patron IDs to keep them visible in the form
            model.BookId = model.BookId;
            model.PatronId = model.PatronId;
        }
    }

    // If validation fails OR checkout fails, return to the view with errors
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


        // ---------------------------------------------------------------------
        // OVERDUE REPORT (GET: /Loan/Overdue)
        // ---------------------------------------------------------------------
        public async Task<IActionResult> Overdue()
        {
            var overdueLoans = await _loanService.GetOverdueLoansAsync();

            return View(overdueLoans);
        }
    }
}