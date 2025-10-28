using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Optional, but good practice

namespace BookLibraryApp.Models.Entities;

public partial class Book
{
    // Existing Properties
    [Key] // Ensure primary key is marked if not done by convention
    public int BookId { get; set; }

    [Required] // Add validation
    [StringLength(200)] // Match DB schema if needed
    public string Title { get; set; } = null!;

    public int? AuthorId { get; set; }

    // --- 👇 NEW METADATA FIELDS 👇 ---

    // Description (Allow long text)
    public string? Description { get; set; }

    // Publication Year
    public int? PublicationYear { get; set; }

    // Genre/Category
    [StringLength(50)] // Add max length if desired
    public string? Genre { get; set; }

    // Path to the cover image file
    [StringLength(500)] // Allow a reasonable path length
    public string? CoverImageUrl { get; set; }

    // --- 👆 END OF NEW FIELDS 👆 ---

    // Navigation Properties
    public virtual Author? Author { get; set; }

    // Navigation to Loans (Assuming you have this relationship)
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}