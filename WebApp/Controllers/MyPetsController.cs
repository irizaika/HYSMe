using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers;

[Authorize]
public class MyPetsController : Controller
{
    private readonly IMyPetsService _myPetsService;


    public MyPetsController(IMyPetsService myPetsService)
    {
        _myPetsService = myPetsService;

    }

    public async Task<IActionResult> Index()
    {
        var vm = new MyPetsViewModel();

        var petsResponse =
            await _myPetsService.GetMyPetsAsync();

        if (petsResponse?.IsSuccess == true &&
            petsResponse.Data != null)
        {
            vm.MyPets = petsResponse.Data
                .Select(MapperHelper.MapToEntity)
                .ToList();
        }

        var sightingsResponse =
            await _myPetsService.GetMySightingsAsync();

        if (sightingsResponse?.IsSuccess == true &&
            sightingsResponse.Data != null)
        {
            vm.MySightings = sightingsResponse.Data
                .Select(MapperHelper.MapToEntity)
                .ToList();
        }

        return View(vm);
    }
}