using Contracts.Enums;

namespace Contracts.Models
{
    public class PetQueryDto
    {
        public int ItemPerPage { get; set; } = 10;
        public int PageNumber { get; set; } = 1;

        public List<PetStatus>? Statuses { get; set; } // multiple statuses filter

        // Optional bounding box for map filtering
        public double? North { get; set; }
        public double? South { get; set; }
        public double? East { get; set; }
        public double? West { get; set; }
    }
}
