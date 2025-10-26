using BookLibraryApp.Models.ViewModels;
using BookLibraryApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryApp.Controllers
{
    [Authorize(Roles = "Admin")]
    // The PatronController will handle all HTTP requests for the Patron module.
    public class PatronController : Controller
    {
        // 1. Dependency Injection: We depend on the IPatronService interface.
        private readonly IPatronService _patronService;

        public PatronController(IPatronService patronService)
        {
            _patronService = patronService;
        }

        // ---------------------------------------------------------------------
        // INDEX (GET: /Patron)
        // ---------------------------------------------------------------------

        // Method is marked 'async' and returns 'Task<IActionResult>'
        public async Task<IActionResult> Index()
        {
            // We 'await' the asynchronous service method call.
            var patrons = await _patronService.GetAllPatrons();

            return View(patrons);
        }

        // ---------------------------------------------------------------------
        // DETAILS (GET: /Patron/Details/5)
        // ---------------------------------------------------------------------

        public async Task<IActionResult> Details(int id)
        {
            var patron = await _patronService.GetPatronById(id);

            if (patron == null)
            {
                return NotFound();
            }

            return View(patron);
        }

        // ---------------------------------------------------------------------
        // CREATE (GET: /Patron/Create)
        // ---------------------------------------------------------------------

        // Returns the empty view for creating a new Patron
        public IActionResult Create()
        {
            return View();
        }

        // ---------------------------------------------------------------------
        // CREATE (POST: /Patron/Create)
        // ---------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber")] PatronViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _patronService.AddPatron(model);
                return RedirectToAction(nameof(Index));
            }
            // If validation fails, return the model back to the view to show errors
            return View(model);
        }

        // ---------------------------------------------------------------------
        // EDIT (GET: /Patron/Edit/5)
        // ---------------------------------------------------------------------

        public async Task<IActionResult> Edit(int id)
        {
            var patron = await _patronService.GetPatronById(id);
            if (patron == null)
            {
                return NotFound();
            }
            return View(patron);
        }

        // ---------------------------------------------------------------------
        // EDIT (POST: /Patron/Edit/5)
        // ---------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatronId,FirstName,LastName,Email,PhoneNumber")] PatronViewModel model)
        {
            if (id != model.PatronId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool success = await _patronService.UpdatePatron(model);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                // If update fails (e.g., patron not found during update), return not found.
                return NotFound();
            }
            return View(model);
        }

        // ---------------------------------------------------------------------
        // DELETE (GET: /Patron/Delete/5)
        // ---------------------------------------------------------------------

        public async Task<IActionResult> Delete(int id)
        {
            var patron = await _patronService.GetPatronById(id);
            if (patron == null)
            {
                return NotFound();
            }

            return View(patron);
        }

        // ---------------------------------------------------------------------
        // DELETE (POST: /Patron/Delete/5)
        // ---------------------------------------------------------------------

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _patronService.DeletePatron(id);
            return RedirectToAction(nameof(Index));
        }
    }
}