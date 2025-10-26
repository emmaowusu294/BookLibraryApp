// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BookLibraryApp.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager; // 🔑 ADDED: To check user roles
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, // 🔑 ADDED: Injection
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager; // 🔑 ASSIGNMENT
            _logger = logger;
        }

        /// <summary>
        ///       This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///       directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///       This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///       directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///       This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///       directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///       This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///       directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///       This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///       directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///       This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///       directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///       This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///       directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///       This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///       directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // 🔑 START OF NEW ROLE-BASED REDIRECTION LOGIC 🔑

                    // 1. Retrieve the user object
                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    if (user != null)
                    {
                        // 2. Check if the user is in the "Admin" role
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            // Admin redirection: Go to the Admin dashboard
                            return LocalRedirect("/Admin/Dashboard");
                        }
                    }

                    // 3. Default redirection for Patrons (non-Admins): Go to the Books Catalog
                    //    This will also handle users not found (though they shouldn't succeed sign-in)
                    return LocalRedirect("/Catalog");

                    // 🔑 END OF NEW ROLE-BASED REDIRECTION LOGIC 🔑
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}