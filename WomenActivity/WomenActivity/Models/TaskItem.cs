using System;
using System.ComponentModel.DataAnnotations;

namespace WomenActivity.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public string Color { get; set; } // Back to string for user-defined input

        [Required]
        public string Type { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
