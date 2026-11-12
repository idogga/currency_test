using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonService.Application.Queries;
using PersonService.Domain.Entities;
using PersonService.Infrastructure.Persistence;

namespace PersonService.Application.Handlers;

/// <summary>
/// Обработчик команды получения списка всех пользователей.
/// </summary>
public class ShowAllUsersCommandHandler : IRequestHandler<ShowAllUsersQuery, IList<User>>
{
    /// <summary>
    /// Контекст базы данных для работы с пользователями.
    /// </summary>
    private readonly AppDbContext _db;

    public ShowAllUsersCommandHandler(AppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<IList<User>> Handle(ShowAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _db.Users.ToListAsync(cancellationToken);
        return users;
    }
}