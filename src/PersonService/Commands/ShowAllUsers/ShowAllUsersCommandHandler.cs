using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonService.Infrastructure.Persistence;
using PersonService.Models;

namespace PersonService.Commands.ShowAllUsers;

/// <summary>
/// Обработчик команды получения списка всех пользователей.
/// </summary>
public class ShowAllUsersCommandHandler : IRequestHandler<ShowAllUsersCommand, IList<User>>
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
    public async Task<IList<User>> Handle(ShowAllUsersCommand request, CancellationToken cancellationToken)
    {
        var users = await _db.Users.ToListAsync(cancellationToken);
        return users;
    }
}