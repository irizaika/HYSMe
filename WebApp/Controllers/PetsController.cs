using Contracts.Enums;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Services.Interfaces;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class PetsController : Controller
    {
        private readonly IPetService _petService;
        private readonly IFileService _fileService;

        public PetsController(IPetService petService, IFileService fileService)
        {
            _petService = petService;
            _fileService = fileService;
        }

        // List all pets
        public async Task<IActionResult> Index()
        {
            List<PetViewModel> list = [];

            var response = await _petService.GetAllPetsAsync();

            if (response != null && response.IsSuccess)
            {
                var json = Convert.ToString(response.Result) ?? "[]";
                var petDtos = JsonConvert.DeserializeObject<List<PetDto>>(json) ?? [];

                list = [..petDtos.Select(MapperHelper.MapToEntity)];
   
            }

        //    ViewBag.CurrentUserId = User.FindFirst("sub")?.Value;
            return View(list);
        }

        //// Details
        //public async Task<IActionResult> Get(int id)
        //{
        //    var response = await _petService.GetPetByIdAsync(id);

        //    if (response != null && response.IsSuccess && response.Result != null)
        //    {
        //        var pet = JsonConvert.DeserializeObject<PetDto>(
        //            Convert.ToString(response.Result)
        //        );

        //        return Ok(response.Result);
        //    }

        //    return NotFound();
        //}

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _petService.GetPetByIdAsync(id);

            if (response != null && response.IsSuccess)
            {
                //var pet = JsonConvert.DeserializeObject<PetDto>(
                //    Convert.ToString(response.Result)
                //);

                var json = Convert.ToString(response.Result) ?? "[]";
                var petDto = JsonConvert.DeserializeObject<PetDto>(json) ?? new PetDto();

                var pet = MapperHelper.MapToEntity(petDto);

                ViewBag.Statuses = Enum.GetValues<PetStatus>()
                  .Cast<PetStatus>()
                  .Select(s => new {
                      Value = (int)s,
                      Text = s.GetDisplayName()
                  });

                return Json(pet);
            }

            return BadRequest();
        }

        // GET Create
        public IActionResult Create()
        {
            return View();
        }

        //// POST Create
        //[HttpPost]
        //public async Task<IActionResult> Create(PetDto model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    var response = await _petService.CreatePetAsync(model);

        //    if (response != null && response.IsSuccess)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    ModelState.AddModelError("", response?.Message ?? "Error");
        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateFromModal( [FromForm] PetViewModel model, [FromForm(Name = "ImageFile")] IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var filePath = await _fileService.SaveImageAsync(imageFile, "images/pets");
            model.ImageUrl = filePath;

            var dto = MapperHelper.MapToDto(model);
            
           // dto.UserId = User.FindFirst("sub")?.Value ?? "";
         //   dto.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!; //not working

            var response = await _petService.CreatePetAsync(dto);

            if (response != null && response.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new { message = response?.Message });
        }

        [HttpPut]
        public async Task<IActionResult> EditFromModal([FromForm] PetViewModel model, [FromForm(Name = "ImageFile")] IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.PetId == null)
                return BadRequest("Invalid ID");


            if (imageFile != null)
            {
                var filePath = await _fileService.SaveImageAsync(imageFile, "images/pets");
                model.ImageUrl = filePath;
            }
            else
            {
                model.ImageUrl = null;
            }

            //  model.UserId = User.FindFirst("sub")?.Value ?? "";

            var pet = MapperHelper.MapToDto(model);

            var response = await _petService.UpdatePetAsync(model.PetId.Value, pet);

            if (response != null && response.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(response?.Message);
        }

        [HttpGet]
        public async Task<IActionResult> GetByBounds(
            double north, double south, double east, double west)
        {
            var area = new AreaDto()
            {
                East = east,
                West = west,
                South = south,
                North = north
            };
            var response = await _petService.GetPetsInArea(area);

            if (response != null && response.IsSuccess)
            {
                //var pets = JsonConvert.DeserializeObject<List<PetDto>>(
                //    Convert.ToString(response.Result)
                //);

                var json = Convert.ToString(response.Result) ?? "[]";
                var petDtos = JsonConvert.DeserializeObject<List<PetDto>>(json) ?? [];

                var pets = petDtos.Select(MapperHelper.MapToEntity);

                return Json(pets);
            }
            return BadRequest(response?.Message);
        }
    }
}