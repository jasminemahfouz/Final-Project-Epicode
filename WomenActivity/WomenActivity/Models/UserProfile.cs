using System;
using System.ComponentModel.DataAnnotations;

namespace WomenActivity.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
        public int Age { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(100)]
        public string? ProfilePhoto { get; set; }

        // Foreign key for User
        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Goal> Goals { get; set; } = new List<Goal>();
        public ICollection<Memory> Memories { get; set; } = new List<Memory>();
        public ICollection<Book> BooksToRead { get; set; } = new List<Book>();
    }
}

