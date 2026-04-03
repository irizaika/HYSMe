using System.ComponentModel.DataAnnotations;

namespace PetsApi.Models
{
    public class Sighting
    {
        public int Id { get; set; }

        [Required]
        public int PetId { get; set; }

        [Required, MaxLength(500)]
        public string Comment { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public DateTime DateSeen { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation
        public Pet Pet { get; set; } = null!;
    }
}