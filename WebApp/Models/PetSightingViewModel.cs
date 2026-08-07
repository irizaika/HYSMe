namespace WebApp.Models
{
    public class PetSightingViewModel

    {
        public int? Id { get; set; }
        public int PetId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? SeenAddress { get; set; }
        public DateTime? DateSeen { get; set; }
        public string? Comment { get; set; }
        public string? ReporterName { get; set; }
        public string? ReporterEmail { get; set; }
        public string? ImageUrl { get; set; } 
        public string? PetName { get; set; }
    }
}
