using MediatR;
using FinanceService.Domain.Interfaces;
using FinanceService.Application.Queries;
using FinanceService.Domain.Entities;

namespace FinanceService.Application.Handlers;

/// <summary>
/// Обработчик запроса на получение списка валют.
/// </summary>
/// <param name="repository"><see cref="ICurrencyRepository"/>.</param>
public sealed class GetCurrenciesHandler(ICurrencyRepository repository)
    : IRequestHandler<GetCurrenciesQuery, IQueryable<Currency>>
{
    /// <summary>
    /// Обработчик запроса на получение списка валют.
    /// </summary>
    /// <param name="request">Запрос на получение списка валют.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список валют.</returns>
    public Task<IQueryable<Currency>> Handle(GetCurrenciesQuery request, CancellationToken ct)
    {
        return Task.FromResult(repository.Query());
    }
}