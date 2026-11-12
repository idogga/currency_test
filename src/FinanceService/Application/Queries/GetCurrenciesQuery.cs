using MediatR;
using FinanceService.Domain.Entities;

namespace FinanceService.Application.Queries;

/// <summary>
/// Запрос для получения списка всех валют.
/// </summary>
public sealed record GetCurrenciesQuery : IRequest<IQueryable<Currency>>;