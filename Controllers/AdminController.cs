using BookLibraryApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// 🔑 IMPORTANT: Commenting out or replacing the ambiguous 'using BookLibraryApp.Data;'
// using BookLibraryApp.Data; 
using System.Linq;
using System.Threading.Tasks;
using System;
using BookLibraryApp.Models.Entities; // This namespace is likely causing the conflict

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // 🔑 FIX: Using the fully qualified name to explicitly define the DbContext type
    private readonly BookLibraryApp.Models.Entities.LibraryDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    // 🔑 FIX: Using the fully qualified name in the constructor argument
    public AdminController(BookLibraryApp.Models.Entities.LibraryDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var model = new AdminDashboardViewModel();

        // --- 1. Core Counts ---
        model.TotalBooks = await _context.Books.CountAsync();

        var activeLoans = await _context.Loans.CountAsync(l => l.IsActive);
        model.TotalActiveLoans = activeLoans;

        // --- Assumption: Count all users for now ---
        model.TotalPatrons = await _userManager.Users.CountAsync();

        // --- 2. Capacity/Availability ---
        model.AvailableBooks = model.TotalBooks - model.TotalActiveLoans;

        // --- 3. Expiring Loans (Next 3 Days) ---
        var threeDaysFromNow = DateTime.Now.AddDays(3);
        model.ExpiringLoansNext3Days = await _context.Loans
            .CountAsync(l => l.IsActive && l.DueDate <= threeDaysFromNow && l.DueDate >= DateTime.Now);

        // --- 4. Most Popular Book ---
        var mostPopularBook = await _context.Loans
            .GroupBy(l => l.BookId)
            .Select(g => new { BookId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(1)
            .Join(_context.Books, loanGroup => loanGroup.BookId, book => book.BookId, (loanGroup, book) => new { book.Title, loanGroup.Count })
            .FirstOrDefaultAsync();

        if (mostPopularBook != null)
        {
            model.MostPopularBookTitle = mostPopularBook.Title;
            model.MostPopularBookLoanCount = mostPopularBook.Count;
        }

        return View(model);
    }
}