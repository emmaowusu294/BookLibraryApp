using System.Diagnostics;
using System.Threading.Tasks; // Required for async/await
using BookLibraryApp.Models;
using Microsoft.AspNetCore.Identity; // Required for UserManager and IdentityUser
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager; // 🔑 ADDED: Declaration

        // 🔑 CORRECTED CONSTRUCTOR: Now includes UserManager
        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager; // 🔑 ADDED: Assignment
        }

        // 🔑 CORRECTED INDEX ACTION: Now performs role-based redirection
        public async Task<IActionResult> Index()
        {
            // CHECK 1: Is the user logged in?
            if (User.Identity.IsAuthenticated)
            {
                // If authenticated, we perform the role check
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    // CHECK 2: Is the user an Admin?
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        // Admin: Redirect to Admin Dashboard
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else {

                        // normal users
                        return RedirectToAction("Index", "Catalog");
                    }
                }

               
            }

            // If not authenticated, display the marketing landing page view
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}