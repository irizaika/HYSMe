//using Microsoft.AspNetCore.Http;
using Contracts.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Models
{
    public class PetDto
    {
        public int? PetId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Breed { get; set; }

        [MaxLength(30)]
        public string? Color { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public DateTime DateLost { get; set; }

        public string? ImageUrl { get; set; }

        //public IFormFile? ImageFile { get; set; }

        public string? LastSeenAddress { get; set; }

        //[Required]
        //public string UserId { get; set; } = string.Empty;

        public PetStatus Status { get; set; }

        public bool IsOwner { get; set; }

        public List<SightingDto> Sightings { get; set; } = new();
    }
}
