using System.ComponentModel.DataAnnotations;

namespace Contracts.Models
{
    public class AreaDto
    {
        [Required]
        public double North { get; set; }

        [Required]
        public double South { get; set; }

        [Required]
        public double West { get; set; }

        [Required]
        public double East { get; set; }
    }
}
