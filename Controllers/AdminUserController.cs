using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BookLibraryApp.Controllers
{
    // Restrict access to only users in the "Admin" role
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminUserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /AdminUser/Index
        // Shows a list of all registered users/patrons in the system
        public async Task<IActionResult> Index()
        {
            // Fetch all users from the Identity system
            var users = await _userManager.Users.ToListAsync();

            // To pass the roles to the view, we'll create a simple list of objects
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }

            // We will pass the list of ViewModels to the Index view
            return View(userViewModels);
        }

        // GET: /AdminUser/Details/{id}
        // Shows details and allows role management for a specific user
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserViewModel
            {
                User = user,
                Roles = roles.ToList()
            };

            return View(model); // Create Views/AdminUser/Details.cshtml
        }

        // POST: /AdminUser/PromoteToAdmin/{id}
        // Example action: Promote a user to Admin role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Check if user is already an Admin to prevent error
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["Success"] = $"{user.Email} has been promoted to Admin.";
            }

            return RedirectToAction("Details", new { id = user.Id });
        }

        // POST: /AdminUser/DeleteUser/{id}
        // Example action: Delete a user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = $"User {user.Email} has been successfully deleted.";
            }
            else
            {
                TempData["Error"] = $"Error deleting user: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction("Index");
        }
    }

    // 🔑 FIX: This ViewModel is now defined OUTSIDE the AdminUserController class,
    // making it publicly accessible within the namespace.
    public class UserViewModel
    {
        public IdentityUser User { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}