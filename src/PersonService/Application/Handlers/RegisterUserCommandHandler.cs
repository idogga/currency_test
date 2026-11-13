using System.Security.Claims;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using PersonService.Application.Dtos;
using PersonService.Commands.Register;
using PersonService.Domain.Entities;
using PersonService.Domain.Interfaces;
using PersonService.Infrastructure.Persistence;

namespace PersonService.Application.Handlers;

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
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;


    public RegisterUserCommandHandler(AppDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService, IConfiguration configuration)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    /// <summary>
    /// Создает пользователя с хэшированным паролем и выдает JWT-токен.
    /// </summary>
    /// <param name="request">Команда с именем пользователя и паролем.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Токен доступа и время его истечения.</returns>
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


        var now = DateTime.UtcNow;
        var expiresAt = now.AddHours(1);
        var signingCredentials = await _jwtService.GetCurrentSigningCredentials();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            SigningCredentials = signingCredentials
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return new AuthResponseDto(token, expiresAt);
    }
}
