using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonService.Dtos;
using PersonService.Infrastructure.Persistence;
using PersonService.Interfaces;
using PersonService.Models;

namespace PersonService.Commands.Register;

/// <summary>
/// Обработчик команды регистрации нового пользователя.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
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

    public RegisterUserCommandHandler(AppDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    /// <inheritdoc />
    public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _db.Users.AnyAsync(u => u.Name == request.Name, cancellationToken))
            throw new InvalidOperationException("User with this name already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Password = _passwordHasher.Hash(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        var token = _jwtService.GenerateToken(user.Id, user.Name, out _, out var expiresAt);
        return new AuthResponseDto(token, expiresAt);
    }
}
