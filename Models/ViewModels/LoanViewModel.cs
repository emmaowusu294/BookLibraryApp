using System.ComponentModel.DataAnnotations;

namespace BookLibraryApp.Models.ViewModels
{
    public class LoanViewModel
    {
        // 1. Core Loan Properties
        public int LoanId { get; set; }

        [Required]
        [Display(Name = "Book")]
        public int BookId { get; set; }

        [Required]
        [Display(Name = "Patron")]
        public int PatronId { get; set; }

        [Display(Name = "Checkout Date")]
        [DataType(DataType.Date)]
        public DateTime CheckoutDate { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; } // Nullable

        // 2. Display Properties (for Index/Details views)
        // These fields are populated by the Service layer, not user input.
        [Display(Name = "Book Title")]
        public string BookTitle { get; set; } = string.Empty;

        [Display(Name = "Patron")]
        public string PatronFullName { get; set; } = string.Empty;

        // 3. Status Property
        [Display(Name = "Status")]
        public string Status
        {
            get
            {
                if (ReturnDate.HasValue)
                {
                    return "Returned";
                }
                else if (DateTime.Now.Date > DueDate.Date)
                {
                    return "OVERDUE";
                }
                else
                {
                    return "Active";
                }
            }
        }
    }
}