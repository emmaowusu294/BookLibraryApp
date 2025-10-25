using System.ComponentModel.DataAnnotations;

namespace BookLibraryApp.Models.ViewModels
{
    public class PatronViewModel
    {
        public int PatronId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(150)]
        public string Email { get; set; }

        [Display(Name = "Phone Number (Optional)")]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
    }
}