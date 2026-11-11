using MediatR;
using PersonService.Dtos;

namespace PersonService.Commands.Login;

/// <summary>
/// Команда для аутентификации пользователя.
/// </summary>
/// <param name="Name">Имя пользователя</param>
/// <param name="Password">Пароль пользователя</param>
public record LoginCommand(string Name, string Password) : IRequest<AuthResponseDto>;