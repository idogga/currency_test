using FinanceService.Domain.Entities;

namespace FinanceService.Domain.Interfaces;

/// <summary>
/// Репозиторий для работы с валютами.
/// </summary>
public interface ICurrencyRepository
{
    /// <summary>
    /// Получает все валюты.
    /// </summary>
    IQueryable<Currency> Query();
}