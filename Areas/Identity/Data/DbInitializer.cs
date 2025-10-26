using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BookLibraryApp.Areas.Identity.Data //  IMPORTANT: Change the namespace if yours is different
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Create Roles if they don't exist
            string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // Note: You can add other roles here (e.g., "Patron", "Librarian")
            // if (!await roleManager.RoleExistsAsync("Librarian"))
            // {
            //     await roleManager.CreateAsync(new IdentityRole("Librarian"));
            // }

            // 2. Create a default Admin User if they don't exist
            string adminEmail = "admin@library.com"; // 📌 Change this email!
            string adminPassword = "AdminPassword123!"; // 📌 Change this password!

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // Assuming we don't need to confirm the default admin
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // 3. Assign the "Admin" role to the new user
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }
        }
    }
}