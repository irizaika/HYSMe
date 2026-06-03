using Contracts.Enums;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using WebApp.Services;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPetService _petService;

        public HomeController(ILogger<HomeController> logger, IPetService petService)
        {
            _logger = logger;
            _petService = petService;
        }

        public async Task<IActionResult> Index()
        {
            List<PetDto> latestLostPetsDtos = [];
            List<PetDto> latestFoundPetsDtos = [];

            var homeFilterForLostPets = new PetQueryDto
            {
                ItemPerPage = 4,
                PageNumber = 1,
                Statuses = [PetStatus.Lost]
            };

            var latestLostPetsResponse = await _petService.QueryPetsAsync(homeFilterForLostPets);
            if (latestLostPetsResponse != null && latestLostPetsResponse.IsSuccess && latestLostPetsResponse.Data != null)
            { 
                latestLostPetsDtos = latestLostPetsResponse.Data.Items;
            }

            var homeFilterForFoundPets = new PetQueryDto
            {
                ItemPerPage = 4,
                PageNumber = 1,
                Statuses = [PetStatus.Reunited, PetStatus.Deceased]
            };

            var latestFoundPetsResponse = await _petService.QueryPetsAsync(homeFilterForFoundPets);
            if (latestFoundPetsResponse != null && latestFoundPetsResponse.IsSuccess && latestFoundPetsResponse.Data != null)
            {
                latestFoundPetsDtos = latestFoundPetsResponse.Data.Items;
            }


            List<PetViewModel> latestLostPets = [.. latestLostPetsDtos.Select(p => MapperHelper.MapToEntity(p))];
            List<PetViewModel> latestFoundPets = [.. latestFoundPetsDtos.Select(p => MapperHelper.MapToEntity(p))];

            var model = new HomeViewModel
            {
                LostPets = latestLostPets ?? [],
                FoundPets = latestFoundPets ?? []
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
