using System;
using System.ComponentModel.DataAnnotations;

namespace WomenActivity.Models
{
    public class DailyWellnessRecord
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Mood { get; set; }

        [Required]
        public int EnergyLevel { get; set; }

        public string Notes { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}

