using System.ComponentModel.DataAnnotations;

namespace Contracts.Models
{
    public class SightingDto
    {
        public int? Id { get; set; }

        [Required]
        public int PetId { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        public DateTime DateSeen { get; set; } = DateTime.UtcNow;

        public string? SeenAddress { get; set; }
        public string? ImageUrl { get; set; }
        public string? ReporterName { get; set; }
        public string? ReporterEmail { get; set; }

    }
}
