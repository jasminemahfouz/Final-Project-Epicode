using System;
using System.ComponentModel.DataAnnotations;

namespace WomenActivity.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is required.")]
        [StringLength(150, ErrorMessage = "The title cannot exceed 150 characters.")]
        public string Title { get; set; }

        [StringLength(100, ErrorMessage = "The author's name cannot exceed 100 characters.")]
        public string Author { get; set; }

        public bool IsRead { get; set; }

        // Foreign key to associate with UserProfile
        [Required]
        public int UserProfileId { get; set; }
        public UserProfile? UserProfile { get; set; }

        // Optional: Add date tracking
        public DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}

