using Contracts.Enums;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            if (latestLostPetsResponse != null && latestLostPetsResponse.IsSuccess)
            {
                var json = Convert.ToString(latestLostPetsResponse.Result) ?? "[]";
                latestLostPetsDtos = JsonConvert.DeserializeObject<List<PetDto>>(json) ?? [];
            }

            var homeFilterForFoundPets = new PetQueryDto
            {
                ItemPerPage = 4,
                PageNumber = 1,
                Statuses = [PetStatus.Reunited, PetStatus.Deceased]
            };

            var latestFoundPetsResponce = await _petService.QueryPetsAsync(homeFilterForFoundPets);
            if (latestFoundPetsResponce != null && latestFoundPetsResponce.IsSuccess)
            {
                var json = Convert.ToString(latestFoundPetsResponce.Result) ?? "[]";
                latestFoundPetsDtos = JsonConvert.DeserializeObject<List<PetDto>>(json) ?? [];
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
