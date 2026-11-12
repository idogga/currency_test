using MediatR;

namespace PersonService.Commands.Logout;

/// <summary>
/// Команда для выхода пользователя из системы.
/// </summary>
/// <param name="Jti">JWT ID.</param>
/// <param name="ExpiresAt">Дата и время истечения токена.</param>
public record LogoutCommand(string Jti, DateTime ExpiresAt) : IRequest<Unit>;