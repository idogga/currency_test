using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using FinanceService.Application.Dtos;
using FinanceService.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using FinanceService.Domain.Entities;

namespace FinanceService.API.Controllers;

/// <summary>
/// Контроллер для работы с валютами.
/// </summary>
/// <param name="mediator"></param>
/// <param name="mapper"></param>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CurrenciesController(IMediator mediator, IMapper mapper) : ODataController
{
    /// <summary>
    /// Получение списка всех валют.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IQueryable<CurrencyDto>> Get(ODataQueryOptions<Currency> options, CancellationToken cancellationToken)
    {
        var currencies = mediator.Send(new GetCurrenciesQuery()).GetAwaiter().GetResult();

        var filtered = options.ApplyTo(currencies) as IQueryable<Currency>
                       ?? currencies;

        return filtered.ProjectTo<CurrencyDto>(mapper.ConfigurationProvider);
    }
}