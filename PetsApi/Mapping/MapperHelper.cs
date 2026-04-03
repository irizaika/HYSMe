using PetsApi.Models;
using Contracts.Enums;
using Contracts.Models;

namespace PetsApi.Mapping
{
    public static class MapperHelper
    {
        public static PetDto MapToDto(Pet p) => new()
        {
            PetId = p.Id,
            Name = p.Name,
            Type = p.Type,
            Breed = p.Breed,
            Color = p.Color,
            Description = p.Description,
            Latitude = p.Latitude,
            Longitude = p.Longitude,
            DateLost = p.DateLost,
            ImageUrl = p.ImageUrl,
            LastSeenAddress = p.LastSeenAddress,
            //UserId = p.UserId,
            Status = p.Status,
            IsOwner = false,// will be changed after, if needed

        };

        public static Pet MapToEntity(PetDto dto) => new()
        {
            Id = dto.PetId ?? 0,//todo
            Name = dto.Name,
            Type = dto.Type,
            Breed = dto.Breed,
            Color = dto.Color,
            Description = dto.Description,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            DateLost = dto.DateLost,
            ImageUrl = dto.ImageUrl,
            LastSeenAddress = dto.LastSeenAddress,
           // UserId = dto.UserId, // to be set in service alyer
            Status = dto.Status
        };


        public static SightingDto MapToDto(Sighting s) => new()
        {
            Id = s.Id,
            PetId = s.PetId,
            Comment = s.Comment,
            ImageUrl = s.ImageUrl,
            Latitude = s.Latitude,
            Longitude = s.Longitude,
            DateSeen = s.DateSeen,
            UserId = s.UserId
        };

        public static Sighting MapToEntity(SightingDto dto) => new()
        {
            Id = dto.Id ?? 0,
            PetId = dto.PetId,
            Comment = dto.Comment,
            ImageUrl = dto.ImageUrl,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            DateSeen = dto.DateSeen,
            UserId = dto.UserId ?? ""
        };
    }
}
