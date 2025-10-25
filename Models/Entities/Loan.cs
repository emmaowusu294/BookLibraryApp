using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibraryApp.Models.Entities
{
    public class Loan
    {
        // Primary Key for the Loan transaction
        [Key]
        public int LoanId { get; set; }

        // Foreign Key to the Book entity
        public int BookId { get; set; }

        // Foreign Key to the Patron entity
        public int PatronId { get; set; }

        // Date the book was checked out (required)
        public DateTime CheckoutDate { get; set; } = DateTime.Now; // Set a default value

        // Date the book is due back (e.g., 14 days after checkout)
        public DateTime DueDate { get; set; }

        // Date the book was returned (null if still checked out)
        public DateTime? ReturnDate { get; set; }

        // --------------------------------------------------------
        // Navigation Properties (Used by EF Core for relationships)
        // --------------------------------------------------------

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; } = null!;

        [ForeignKey("PatronId")]
        public virtual Patron Patron { get; set; } = null!;
    }
}