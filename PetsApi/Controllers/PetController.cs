using Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsApi.Data;
using PetsApi.Mapping;
using PetsApi.Models;

namespace PetsApi.Controllers
{
    [Route("api/pets")]
    [ApiController]
    [Authorize]
    public class PetController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PetController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var pets = await _db.Pets.ToListAsync();
                var userId = User.FindFirst("sub")?.Value;

                var petsDto = pets.Select(p =>
                {
                    var dto = MapperHelper.MapToDto(p);
                    dto.IsOwner = p.UserId == userId;
                    return dto;
                }).ToList();

                return Ok(ApiResponse<List<PetDto>>.Success(petsDto));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }

        [HttpGet("query")]
        [AllowAnonymous]
        public async Task<IActionResult> QueryPets([FromQuery] PetQueryDto query)
        {
            try
            {
                IQueryable<Pet> pets = _db.Pets;

                // Filter by statuses
                if (query.Statuses != null && query.Statuses.Count > 0)
                {
                    pets = pets.Where(p => query.Statuses.Contains(p.Status));
                }

                // Filter by bounding box
                if (query.North.HasValue && query.South.HasValue && query.East.HasValue && query.West.HasValue)
                {
                    pets = pets.Where(p =>
                        p.Latitude >= query.South &&
                        p.Latitude <= query.North &&
                        p.Longitude >= query.West &&
                        p.Longitude <= query.East);
                }

                // Order by latest lost/found
                pets = pets.OrderByDescending(p => p.DateLost);

                // Pagination
                var pagedPets = await pets
                    .Skip((query.PageNumber - 1) * query.ItemPerPage)
                    .Take(query.ItemPerPage)
                    .ToListAsync();

                var userId = User.FindFirst("sub")?.Value;
                var result = pagedPets.Select(p =>
                {
                    var dto = MapperHelper.MapToDto(p);
                    dto.IsOwner = p.UserId == userId;
                    return dto;
                }).ToList();
                return Ok(ApiResponse<List<PetDto>>.Success(result));
              //  _response.Result = result;
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }



        [HttpGet("area")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByArea([FromQuery] AreaDto area)
        {
            try
            {
                var pets = await _db.Pets
                    .Where(p =>
                        p.Latitude >= area.South &&
                        p.Latitude <= area.North &&
                        p.Longitude >= area.West &&
                        p.Longitude <= area.East)
                    .ToListAsync();

                var userId = User.FindFirst("sub")?.Value;

                var result  = pets.Select(p =>
                {
                    var dto = MapperHelper.MapToDto(p);
                    dto.IsOwner = p.UserId == userId;
                    return dto;
                }).ToList();
                return Ok(ApiResponse<List<PetDto>>.Success(result));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var pet = await _db.Pets
                    .Include(p => p.Sightings)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pet == null)
                {
                    return Ok(ApiResponse<PetDto>.Fail(null, "Pet not found"));
                }
                var userId = User.FindFirst("sub")?.Value;

                 var dto = MapperHelper.MapToDto(pet);
                dto.IsOwner = pet.UserId == userId;

                return Ok(ApiResponse<PetDto>.Success(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PetDto dto)
        {
            try
            {
                var pet = MapperHelper.MapToEntity(dto);

                pet.UserId = User.FindFirst("sub")?.Value;

                _db.Pets.Add(pet);
                await _db.SaveChangesAsync();

                var userId = User.FindFirst("sub")?.Value;
                var petDto = MapperHelper.MapToDto(pet);
                petDto.IsOwner = pet.UserId == userId;

                return Ok(ApiResponse<PetDto>.Success(petDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] PetDto dto)
        {
            try
            {
                var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == id);

                if (pet == null)
                {
                    return Ok(ApiResponse<PetDto>.Fail(null, "Pet not found"));
                }

                var userId = User.FindFirst("sub")?.Value;
                if (pet.UserId != userId)
                {
                    return Ok(ApiResponse<PetDto>.Fail(null, "You are not allowed to update this pet"));
                }

                // update fields
                pet.Name = dto.Name;
                pet.Type = dto.Type;
                pet.Breed = dto.Breed;
                pet.Color = dto.Color;
                pet.Description = dto.Description;
                pet.Latitude = dto.Latitude;
                pet.Longitude = dto.Longitude;
                pet.DateLost = dto.DateLost;
                pet.LastSeenAddress = dto.LastSeenAddress;
                pet.Status = dto.Status;

                // do not update new picture was not added TODO add possibility to delete picture, or add mulitple pictures
                if (dto.ImageUrl != null)
                {
                    pet.ImageUrl = dto.ImageUrl;
                }

                await _db.SaveChangesAsync();

                var result = MapperHelper.MapToDto(pet);

                return Ok(ApiResponse<PetDto>.Success(result));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == id);

                if (pet == null)
                {
                    return Ok(ApiResponse<PetDto>.Fail(null, "Pet not found"));
                }

                //only admin can delete records
                //var userId = User.FindFirst("sub")?.Value;
                //if (pet.UserId != userId)
                //{
                //    _response.IsSuccess = false;
                //    _response.Message = "You are not allowed to delete this pet";
                //    return _response;
                //}

                _db.Pets.Remove(pet);
                await _db.SaveChangesAsync();

                return Ok(ApiResponse<PetDto>.Success());
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Error>>.Fail(null, ex.Message));
            }
        }
    }
}
