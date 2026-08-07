using Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsApi.Data;
using PetsApi.Mapping;

namespace PetsApi.Controllers
{
    [Route("api/mypets")]
    [ApiController]
    [Authorize]
    public class MyPetsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MyPetsController(AppDbContext db)
        {
            _db = db;
        }


        [HttpGet("my-pets")]
        public async Task<IActionResult> GetMyPets()
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var pets = await _db.Pets
                    .Where(p => p.UserId == userId)
                    .ToListAsync();

                var result = pets.Select(MapperHelper.MapToDto).ToList();

                return Ok(ApiResponse<List<PetDto>>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }
        [HttpGet("my-sightings")]
        public async Task<IActionResult> GetMySightings()
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var sightings = await _db.Sightings
                    .Include(s => s.Pet)
                    .Where(s => s.UserId == userId)
                    .ToListAsync();

                var result = sightings.Select(MapperHelper.MapToDto).ToList();

                return Ok(ApiResponse<List<SightingDto>>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }
    }
}
