using MediatR;
using PersonService.Commands.Logout;
using PersonService.Domain.Interfaces;

namespace PersonService.Application.Handlers;

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

    /// <summary>
    /// Добавляет JWT пользователя в черный список.
    /// </summary>
    /// <param name="request">Команда с идентификатором и сроком действия токена.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Завершенная задача.</returns>
    public Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _blacklist.AddToBlacklist(request.Jti, request.ExpiresAt);
        return Task.FromResult(Unit.Value);
    }
}