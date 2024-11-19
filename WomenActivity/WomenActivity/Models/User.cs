using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WomenActivity.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        // Collection of tasks related to the user
        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

        // Navigation property for UserProfile (One-to-One relationship)
        public UserProfile? UserProfile { get; set; }
    }
}
