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

        public string? SeenAddress { get; set; }
        public DateTime DateSeen { get; set; } = DateTime.UtcNow;

        // optional for auth users
        public string? UserId { get; set; }

        // optional two fields (for non-auth users)
        public string? ReporterName { get; set; }
        public string? ReporterEmail { get; set; }

        // Navigation
        public Pet Pet { get; set; } = null!;
    }
}