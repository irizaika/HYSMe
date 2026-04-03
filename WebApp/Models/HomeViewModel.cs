using Contracts.Models;

namespace WebApp.Models
{
    public class HomeViewModel
    {
        public List<PetViewModel> LostPets { get; set; } = [];
        public List<PetViewModel> FoundPets { get; set; } = [];
    }
}
