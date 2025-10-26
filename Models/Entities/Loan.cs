using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Required for IdentityUser

namespace BookLibraryApp.Models.Entities
{
    // Note: If you don't have an "Entities" subfolder, adjust the namespace
    public class Loan
    {
        // Primary Key for the Loan transaction
        [Key]
        public int Id { get; set; } // Renamed for common convention

        // Foreign Key to the Book entity
        public int BookId { get; set; }

        // 🔑 REPLACEMENT: Foreign Key to the IdentityUser (the Patron/Reader)
        // IdentityUser IDs are strings (GUIDs)
        public required string UserId { get; set; }

        // Date the digital access started
        public DateTime LoanDate { get; set; } = DateTime.Now;

        // Date the digital access is due to expire
        public DateTime DueDate { get; set; }

        // 🔑 REMOVED: ReturnDate is no longer needed for digital access, 
        // we use IsActive to determine current access.

        // Status to track if access is currently active (e.g., within the 14-day window)
        public bool IsActive { get; set; } = true;

        // --------------------------------------------------------
        // Navigation Properties 
        // --------------------------------------------------------

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; } = null!;

        // 🔑 REPLACEMENT: Link to the IdentityUser
        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; } = null!;
    }
}