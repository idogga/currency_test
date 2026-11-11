using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonService.Dtos;
using PersonService.Infrastructure.Persistence;
using PersonService.Interfaces;

namespace PersonService.Commands.Login;

/// <summary>
/// Обработчик команды входа пользователя в систему.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    /// <summary>
    /// Контекст базы данных для работы с пользователями.
    /// </summary>
    private readonly AppDbContext _db;

    /// <summary>
    /// Сервис для хэширования паролей.
    /// </summary>
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Сервис для генерации JWT-токенов.
    /// </summary>
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(AppDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    /// <inheritdoc />
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .SingleOrDefaultAsync(u => u.Name == request.Name, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _jwtService.GenerateToken(user.Id, user.Name, out _, out var expiresAt);
        return new AuthResponseDto(token, expiresAt);
    }
}