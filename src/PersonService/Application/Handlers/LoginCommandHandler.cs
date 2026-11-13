using System.Security.Claims;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using PersonService.Application.Dtos;
using PersonService.Commands.Login;
using PersonService.Domain.Interfaces;
using PersonService.Infrastructure.Persistence;

namespace PersonService.Application.Handlers;

/// <summary>
/// Обработчик команды входа пользователя в систему.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    /// <summary>
    /// Контекст базы данных для работы с пользователями.
    /// </summary>
    private readonly AppDbContext _db;

    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(AppDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService,
        IConfiguration configuration)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    /// <summary>
    /// Проверяет учетные данные пользователя и создает JWT-токен.
    /// </summary>
    /// <param name="request">Команда с именем пользователя и паролем.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Токен доступа и время его истечения.</returns>
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Name == request.Name, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

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