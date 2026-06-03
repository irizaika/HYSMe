namespace WebApp.Models
{
    public class PetsPageViewModel
    {
        public List<PetViewModel> Pets { get; set; } = new();
        public int CurrentPage { get; set; } 
        public int TotalPages { get; set; } 
        public string Search  { get; set; } = string.Empty;
    }
}
