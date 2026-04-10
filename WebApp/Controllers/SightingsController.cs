using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    public class SightingsController : Controller
    {
        private readonly IPetSightingService _petSightingService;
        private readonly IFileService _fileService;

        public SightingsController(IPetSightingService petSightingService, IFileService fileService)
        {
            _petSightingService = petSightingService;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSighting([FromForm] PetSightingViewModel model, [FromForm(Name = "ImageFile")] IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var filePath = await _fileService.SaveImageAsync(imageFile, "images/pets");
            model.ImageUrl = filePath;

            var dto = new SightingDto
            {
                PetId = model.PetId,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                SeenAddress = model.SeenAddress,
                Comment = model.Comment,
                ReporterName = model.ReporterName,
                ReporterEmail = model.ReporterEmail,
                DateSeen = model.DateSeen.Value,
            };

            var userId = User.FindFirst("sub")?.Value;


            var response = await _petSightingService.CreatePetSightingAsync(dto);

            if (response != null && response.IsSuccess)
                return Ok();

            return BadRequest(new { message = response?.Message });
        }
    }
}
