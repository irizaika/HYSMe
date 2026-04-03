using Contracts.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class PetViewModel
    {
        public int? PetId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        public string? Breed { get; set; }

        public string? Color { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public DateTime? DateLost { get; set; }

        public string? ImageUrl { get; set; }

        public string? LastSeenAddress { get; set; }

        public PetStatus Status { get; set; }

        public bool IsOwner { get;  set; }

        //public bool IsOwner { get; private set; }

        //public void SetOwnership(bool isOwner)
        //{
        //    IsOwner = isOwner;
        //}
    }
}
