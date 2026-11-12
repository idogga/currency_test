using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Repositories;

/// <inheritdoc />
public sealed class CurrencyRepository(AppDbContext db) : ICurrencyRepository
{
    /// <inheritdoc />
    public IQueryable<Currency> Query() => db.Currencies.AsNoTracking();
}