using System;
using System.ComponentModel.DataAnnotations;

namespace WomenActivity.Models
{
    public class Routine
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // Nome della routine

        [Required]
        public TimeSpan Time { get; set; }  // Orario della routine

        [Required]
        public FrequencyType Frequency { get; set; }  // Frequenza della routine

        [Required]
        public RoutineType RoutineType { get; set; }  // Tipo di routine (con icone)

        [Required]
        public DateTime Date { get; set; }  // Data della routine


        // Collegamento all'utente
        public int UserId { get; set; }
        public User? User { get; set; }
    }

    // Enum per la frequenza della routine
    public enum FrequencyType
    {
        Daily,
        Weekly,
        Monthly
    }

    // Enum per il tipo di routine (con icone associate)
    public enum RoutineType
    {
        Shower,
        Exercise,
        School,
        Study,
        Work,
        Other
    }
}
