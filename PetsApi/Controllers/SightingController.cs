using PetsApi.Data;
using PetsApi.Mapping;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetsApi.Controllers
{
    [Route("api/sightings")]
    [ApiController]
    public class SightingController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ResponseDto _response;

        public SightingController(AppDbContext db)
        {
            _db = db;
            _response = new ResponseDto();
        }

        // Get all sightings for a specific pet
        [HttpGet("pet/{petId:int}")]
        public async Task<ResponseDto> GetByPet(int petId)
        {
            try
            {
                var sightings = await _db.Sightings
                    .Where(s => s.PetId == petId)
                    .ToListAsync();

                _response.Result = sightings.Select(MapperHelper.MapToDto);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // Get single sighting
        [HttpGet("{id:int}")]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                var sighting = await _db.Sightings
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (sighting == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Sighting not found";
                    return _response;
                }

                _response.Result = MapperHelper.MapToDto(sighting);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // Create new sighting
        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] SightingDto dto)
        {
            try
            {
                // validate pet exists
                var petExists = await _db.Pets.AnyAsync(p => p.Id == dto.PetId);

                if (!petExists)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid PetId";
                    return _response;
                }

                var sighting = MapperHelper.MapToEntity(dto);

                _db.Sightings.Add(sighting);
                await _db.SaveChangesAsync();

                _response.Result = MapperHelper.MapToDto(sighting);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // Update sighting
        [HttpPut("{id:int}")]
        public async Task<ResponseDto> Put(int id, [FromBody] SightingDto dto)
        {
            try
            {
                var sighting = await _db.Sightings
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (sighting == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Sighting not found";
                    return _response;
                }

                sighting.Comment = dto.Comment;
                sighting.ImageUrl = dto.ImageUrl;
                sighting.Latitude = dto.Latitude;
                sighting.Longitude = dto.Longitude;
                sighting.DateSeen = dto.DateSeen;

                await _db.SaveChangesAsync();

                _response.Result = MapperHelper.MapToDto(sighting);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // Delete sighting
        [HttpDelete("{id:int}")]
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                var sighting = await _db.Sightings
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (sighting == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Sighting not found";
                    return _response;
                }

                _db.Sightings.Remove(sighting);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
    }
}