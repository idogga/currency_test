using MediatR;
using PersonService.Application.Dtos;

namespace PersonService.Commands.Register;

/// <summary>
/// Команда для регистрации нового пользователя.
/// </summary>
/// <param name="Name">Имя пользователя.</param>
/// <param name="Password">Пароль пользователя.</param>
public record RegisterUserCommand(string Name, string Password) : IRequest<AuthResponseDto>;
