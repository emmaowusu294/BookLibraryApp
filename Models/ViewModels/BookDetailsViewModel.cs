using System.ComponentModel.DataAnnotations;

namespace BookLibraryApp.Models.ViewModels
{
    // Inherit from the base model and add the loan status property
    public class BookDetailsViewModel : BookViewModel
    {
        // 🔑 NEW PROPERTY: Flag to indicate if the current user has an active loan
        public bool HasActiveLoan { get; set; }
    }
}