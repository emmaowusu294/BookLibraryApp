using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookLibraryApp.Controllers
{
    // This controller handles redirection for all logged-in users.
    [Authorize] // Only authenticated users can access this controller
    public class UserRedirectController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserRedirectController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // This action will be the new target for the main navbar link.
        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // 1. Check if the user is in the "Admin" role
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    // Redirect Admin to the Admin Dashboard
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    //normal users
                    return RedirectToAction("Index", "Catalog");
                }
            }

            // 2. Default redirection for public
            return RedirectToAction("Index", "Home");
        }
    }
}