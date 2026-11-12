using Microsoft.EntityFrameworkCore;
using PersonService.Domain.Entities;

namespace PersonService.Infrastructure.Persistence;

/// <summary>
/// Контекст базы данных.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Набор пользователей в базе данных.
    /// </summary>
    public DbSet<User> Users => Set<User>();
}