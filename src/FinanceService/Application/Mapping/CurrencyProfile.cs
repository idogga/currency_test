using AutoMapper;
using FinanceService.Domain.Entities;
using FinanceService.Application.Dtos;

namespace FinanceService.Application.Mapping;

/// <summary>
/// Профиль для маппинга между <see cref="Currency"/> и <see cref="CurrencyDto"/>.
/// </summary>
public sealed class CurrencyProfile : Profile
{
    public CurrencyProfile()
    {
        CreateMap<Currency, CurrencyDto>();
    }
}