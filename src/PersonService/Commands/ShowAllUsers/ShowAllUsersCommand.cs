using MediatR;
using PersonService.Dtos;
using PersonService.Models;

namespace PersonService.Commands.ShowAllUsers;

/// <summary>
/// Команда для получения списка всех пользователей.
/// </summary>
public record ShowAllUsersCommand() : IRequest<IList<User>>;