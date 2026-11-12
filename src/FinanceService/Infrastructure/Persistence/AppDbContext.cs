using Microsoft.EntityFrameworkCore;
using FinanceService.Domain.Entities;

namespace FinanceService.Infrastructure.Persistence;

/// <summary>
/// Контекст базы данных.
/// </summary>
/// <param name="options"></param>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// DbSet для работы с сущностью <see cref="Currency"/>.
    /// </summary>
    public DbSet<Currency> Currencies => Set<Currency>();
}