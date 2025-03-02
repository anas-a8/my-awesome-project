using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; }

        public int CopiesAvailable { get; set; }

        public int TimesBorrowed { get; set; } // For ranking most borrowed books
    }
}
