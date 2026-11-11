using AutoMapper;
using PersonService.Dtos;
using PersonService.Models;

namespace PersonService.Common.Mapping;

/// <summary>
/// Профиль для настройки маппинга между сущностями и DTO.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}