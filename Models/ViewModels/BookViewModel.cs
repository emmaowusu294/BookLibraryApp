using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Required for IFormFile

namespace BookLibraryApp.Models.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(150, ErrorMessage = "Title can't be longer than 150 characters")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Author name is required")]
        [StringLength(100, ErrorMessage = "Author name can't be longer than 100 characters")]
        public string? AuthorName { get; set; }

        // Optional — only used internally to map to Author entity
        public int? AuthorId { get; set; }

        // --- 👇 NEW PROPERTIES MATCHING THE ENTITY 👇 ---

        [StringLength(2000)] // Allow longer text for descriptions
        [Display(Name = "Summary/Description")]
        public string? Description { get; set; }

        [Range(1800, 2099)] // Basic year validation
        [Display(Name = "Publication Year")]
        public int? PublicationYear { get; set; }

        [StringLength(50)]
        public string? Genre { get; set; }

        // Store the relative URL/path to the image for display
        [Display(Name = "Cover Image Path")]
        public string? CoverImageUrl { get; set; }

        // Handle the file upload from the HTML form
        [Display(Name = "Upload Cover Image")]
        public IFormFile? CoverImageFile { get; set; }

        // --- 👆 END OF NEW PROPERTIES 👆 ---
    }
}