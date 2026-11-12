using MediatR;
using PersonService.Domain.Entities;

namespace PersonService.Application.Queries;

/// <summary>
/// Команда для получения списка всех пользователей.
/// </summary>
public record ShowAllUsersQuery() : IRequest<IList<User>>;