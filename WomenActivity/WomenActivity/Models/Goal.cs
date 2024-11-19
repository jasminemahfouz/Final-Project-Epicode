using System;
using System.ComponentModel.DataAnnotations;

namespace WomenActivity.Models
{
    public class Goal
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        // Foreign key per UserProfile
        public int UserProfileId { get; set; }

        // Navigazione opzionale per UserProfile
        public UserProfile? UserProfile { get; set; } // Rendi nullable
    }

}

