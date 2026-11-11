using MediatR;
using PersonService.Interfaces;

namespace PersonService.Commands.Logout;

/// <summary>
/// Обработчик команды выхода пользователя из системы.
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    /// <summary>
    /// Сервис для работы с черным списком токенов.
    /// </summary>
    private readonly ITokenBlacklistService _blacklist;

    public LogoutCommandHandler(ITokenBlacklistService blacklist) => _blacklist = blacklist;

    /// <inheritdoc />
    public Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _blacklist.AddToBlacklist(request.Jti, request.ExpiresAt);
        return Task.FromResult(Unit.Value);
    }
}