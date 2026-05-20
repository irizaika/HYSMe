using Azure;
using Contracts.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsApi.Data;
using PetsApi.Mapping;

namespace PetsApi.Controllers
{
    [Route("api/sightings")]
    [ApiController]
    public class SightingController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SightingController(AppDbContext db)
        {
            _db = db;
        }

        // Get all sightings for a specific pet
        [HttpGet("pet/{petId:int}")]
        public async Task<IActionResult> GetByPet(int petId)
        {
            try
            {
                var sightings = await _db.Sightings
                    .Where(s => s.PetId == petId)
                    .ToListAsync();

                var result = sightings.Select(MapperHelper.MapToDto).ToList();

                return Ok(ApiResponse<List<SightingDto>>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }

        // Get single sighting
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var sighting = await _db.Sightings
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (sighting == null)
                {
                    return BadRequest(ApiResponse<List<Error>>.Fail(null, "Sighting not found"));
                }

                var result = MapperHelper.MapToDto(sighting);

                return Ok(ApiResponse<SightingDto>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }

        // Create new sighting
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SightingDto dto)
        {
            try
            {
                // validate pet exists
                var petExists = await _db.Pets.AnyAsync(p => p.Id == dto.PetId);

                if (!petExists)
                {
                    return BadRequest(ApiResponse<List<Error>>.Fail(null, "Invalid PetId"));
                }

                var sighting = MapperHelper.MapToEntity(dto);

                _db.Sightings.Add(sighting);
                await _db.SaveChangesAsync();
                
                var result = MapperHelper.MapToDto(sighting);

                return Ok(ApiResponse<SightingDto>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }

        // Update sighting
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] SightingDto dto)
        {
            try
            {
                var sighting = await _db.Sightings
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (sighting == null)
                {
                    return BadRequest(ApiResponse<List<Error>>.Fail(null, "Sighting not found"));
                }

                sighting.Comment = dto.Comment??"";
                sighting.ImageUrl = dto.ImageUrl;
                sighting.Latitude = dto.Latitude;
                sighting.Longitude = dto.Longitude;
                sighting.DateSeen = dto.DateSeen;

                await _db.SaveChangesAsync();

                var result = MapperHelper.MapToDto(sighting);

                return Ok(ApiResponse<SightingDto>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }

        // Delete sighting
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var sighting = await _db.Sightings
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (sighting == null)
                {
                    return BadRequest(ApiResponse<List<Error>>.Fail(null, "Sighting not found"));
                }

                _db.Sightings.Remove(sighting);
                await _db.SaveChangesAsync();

                return Ok(ApiResponse<SightingDto>.Success());

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }
    }
}