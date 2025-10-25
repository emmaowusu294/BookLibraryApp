using System.ComponentModel.DataAnnotations;

namespace BookLibraryApp.Models.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(150, ErrorMessage = "Title can't be longer than 150 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author name is required")]
        [StringLength(100, ErrorMessage = "Author name can't be longer than 100 characters")]
        public string AuthorName { get; set; }

        // Optional — only used internally to map to Author entity
        public int? AuthorId { get; set; }
    }
}
