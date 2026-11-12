using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using FinanceService.Application.Dtos;
using FinanceService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.API.Controllers;

/// <summary>
/// Контроллер для работы с валютами.
/// </summary>
/// <param name="mediator"></param>
/// <param name="mapper"></param>
[Authorize]
public sealed class CurrenciesController(IMediator mediator, IMapper mapper) : ODataController
{
    /// <summary>
    /// Получение списка всех валют.
    /// </summary>
    /// <returns></returns>
    [EnableQuery]
    [HttpGet]
    public IQueryable<CurrencyDto> Get(CancellationToken cancellationToken)
    {
        var currencies = mediator.Send(new GetCurrenciesQuery(), cancellationToken).GetAwaiter().GetResult();
        return currencies.ProjectTo<CurrencyDto>(mapper.ConfigurationProvider);
    }
}