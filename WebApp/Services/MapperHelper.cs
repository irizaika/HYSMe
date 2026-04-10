using Contracts.Enums;
using Contracts.Models;
using WebApp.Models;

namespace WebApp.Services
{
    public static class MapperHelper
    {
        public static PetDto MapToDto(PetViewModel vm) => new()
        {
            PetId = vm.PetId,
            Name = vm.Name,
            Type = vm.Type,
            Breed = vm.Breed,
            Color = vm.Color,
            Description = vm.Description,
            Latitude = vm.Latitude,
            Longitude = vm.Longitude,
            DateLost = vm.DateLost ?? DateTime.UtcNow,
            ImageUrl = vm.ImageUrl,
            LastSeenAddress = vm.LastSeenAddress,
            Status = vm.Status,
            IsOwner = vm.IsOwner,
        };

        public static PetViewModel MapToEntity(PetDto dto) => new()
        {
            PetId = dto.PetId ?? 0,//todo
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
            Status = dto.Status,
            IsOwner = dto.IsOwner,
            Sightings = dto.Sightings.Select(MapToEntity).ToList()
        };

        public static LoginViewModel MapToEntity(LoginRequestDto dto) => new()
        {
            UserName = dto.UserName,
            Password = dto.Password
        };

        public static LoginRequestDto MapToDto(LoginViewModel dto) => new()
        {
            UserName = dto.UserName,
            Password = dto.Password
        };

        public static RegistrationViewModel MapToEntity(RegistrationRequestDto dto) => new()
        {
            Email = dto.Email,
            Role = dto.Role,
            Name = dto.Name,
            Password = dto.Password,
            PhoneNumber = dto.PhoneNumber
        };

        public static RegistrationRequestDto MapToDto(RegistrationViewModel dto) => new()
        {
            Email = dto.Email,
            Role = Role.RoleUser,// users can be registered only with role USER
            Name = dto.Name,
            Password = dto.Password,
            PhoneNumber = dto.PhoneNumber
        };

        public static SightingDto MapToDto(PetSightingViewModel vm) => new()
        {
            Id = vm.Id,
            PetId = vm.PetId,
            Comment = vm.Comment,
            Latitude = vm.Latitude,
            Longitude = vm.Longitude,
            DateSeen = vm.DateSeen ?? DateTime.UtcNow,
            ImageUrl = vm.ImageUrl,
            SeenAddress = vm.SeenAddress,
            ReporterEmail = vm.ReporterEmail,
            ReporterName = vm.ReporterName
        };

        public static PetSightingViewModel MapToEntity(SightingDto dto) => new()
        {
            Id = dto.Id,
            PetId = dto.PetId,
            Comment = dto.Comment,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            DateSeen = dto.DateSeen,
            ImageUrl = dto.ImageUrl,
            SeenAddress = dto.SeenAddress,
            ReporterEmail = dto.ReporterEmail,
            ReporterName = dto.ReporterName
        };
    }
}
