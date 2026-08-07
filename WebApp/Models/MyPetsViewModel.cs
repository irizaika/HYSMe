namespace WebApp.Models
{
    public class MyPetsViewModel
    {
        public List<PetViewModel> MyPets { get; set; } = [];
        public List<PetSightingViewModel> MySightings { get; set; } = [];
    }
}
