using System.ComponentModel.DataAnnotations;

namespace BookLibraryApp.Models.Entities
{
    public class Patron
    {
        public int PatronId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        //public DateTime DateRegistered { get; set; }
        // Future Use: You might add things like 

        // public MembershipStatus Status { get; set; }
    }
}