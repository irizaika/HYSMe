using System.ComponentModel.DataAnnotations;
using Contracts.Enums;

namespace PetsApi.Models
{
    public class Pet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // Dog, Cat, etc.

        [MaxLength(50)]
        public string? Breed { get; set; }

        [MaxLength(30)]
        public string? Color { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [Required]
        public DateTime DateLost { get; set; }

        // 📍 Location (simple version for SQLite)
        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        [Required]
        public PetStatus Status { get; set; } = PetStatus.Lost;

        [Required]
        public string UserId { get; set; } = string.Empty; // from Auth service

        [MaxLength(200)]
        public string? LastSeenAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Sighting> Sightings { get; set; } = [];
    }
}

