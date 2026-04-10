using PetsApi.Data;
using PetsApi.Mapping;
using PetsApi.Models;
using Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetsApi.Controllers
{
    [Route("api/pets")]
    [ApiController]
    [Authorize]
    public class PetController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ResponseDto _response;

        public PetController(AppDbContext db)
        {
            _db = db;
            _response = new ResponseDto();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ResponseDto> Get()
        {
            try
            {
                var pets = await _db.Pets.ToListAsync();
                var userId = User.FindFirst("sub")?.Value;

                _response.Result = pets.Select(p =>
                {
                    var dto = MapperHelper.MapToDto(p);
                    dto.IsOwner = p.UserId == userId;
                    return dto;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet("query")]
        [AllowAnonymous]
        public async Task<ResponseDto> QueryPets([FromQuery] PetQueryDto query)
        {
            try
            {
                IQueryable<Pet> pets = _db.Pets;

                // Filter by statuses
                if (query.Statuses != null && query.Statuses.Any())
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
                });

                _response.Result = result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }



        [HttpGet("area")]
        [AllowAnonymous]
        public async Task<ResponseDto> GetByArea([FromQuery] AreaDto area)
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

                _response.Result = pets.Select(p =>
                {
                    var dto = MapperHelper.MapToDto(p);
                    dto.IsOwner = p.UserId == userId;
                    return dto;
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                var pet = await _db.Pets
                    .Include(p => p.Sightings)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pet == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Pet not found";
                    return _response;
                }
                var userId = User.FindFirst("sub")?.Value;

                 var dto = MapperHelper.MapToDto(pet);
                dto.IsOwner = pet.UserId == userId;

                _response.Result = dto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] PetDto dto)
        {
            try
            {
                var pet = MapperHelper.MapToEntity(dto);

                _db.Pets.Add(pet);
                await _db.SaveChangesAsync();

                var userId = User.FindFirst("sub")?.Value;
                var petDto = MapperHelper.MapToDto(pet);
                petDto.IsOwner = pet.UserId == userId;

                _response.Result = petDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPut("{id:int}")]
        public async Task<ResponseDto> Put(int id, [FromBody] PetDto dto)
        {
            try
            {
                var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == id);

                if (pet == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Pet not found";
                    return _response;
                }

                var userId = User.FindFirst("sub")?.Value;
                if (pet.UserId != userId)
                {
                    _response.IsSuccess = false;
                    _response.Message = "You are not allowed to update this pet";
                    return _response;
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

                _response.Result = MapperHelper.MapToDto(pet);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == id);

                if (pet == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Pet not found";
                    return _response;
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