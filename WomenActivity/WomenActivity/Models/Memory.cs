using System;
using System.ComponentModel.DataAnnotations;

namespace WomenActivity.Models
{
    public class Memory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il titolo è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il titolo non può superare i 50 caratteri.")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "La data è obbligatoria.")]
        public DateTime Date { get; set; }

        [Url(ErrorMessage = "L'URL dell'immagine non è valido.")]
        public string ImageUrl { get; set; }

        // Foreign key to associate with UserProfile
        public int UserProfileId { get; set; }

        public UserProfile? UserProfile { get; set; }
    }

}

