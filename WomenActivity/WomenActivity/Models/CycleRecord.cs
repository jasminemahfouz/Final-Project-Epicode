using System;
using System.ComponentModel.DataAnnotations;

namespace WomenActivity.Models
{
    public class CycleRecord
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Mood { get; set; } // Added Mood property

        [Required]
        public int PainLevel { get; set; } // Added PainLevel property

        [Required]
        public string Symptoms { get; set; } // Added Symptoms property

        public int UserId { get; set; }
        public User? User { get; set; }
    }

    public class CyclePrediction // Added CyclePrediction model
    {
        public DateTime PredictedStartDate { get; set; }
        public DateTime PredictedEndDate { get; set; }
    }
}