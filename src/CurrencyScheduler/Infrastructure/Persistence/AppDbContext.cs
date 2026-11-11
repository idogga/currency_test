using Microsoft.EntityFrameworkCore;
using CurrencyScheduler.Models;

namespace CurrencyScheduler.Infrastructure.Persistence;

/// <summary>
/// Контекст базы данных.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Набор валют в базе данных.
    /// </summary>
    public DbSet<Currency> Currencies => Set<Currency>();
}