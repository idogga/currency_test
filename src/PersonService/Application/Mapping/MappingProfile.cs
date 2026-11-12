using AutoMapper;
using PersonService.Application.Dtos;
using PersonService.Domain.Entities;

namespace PersonService.Application.Mapping;

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