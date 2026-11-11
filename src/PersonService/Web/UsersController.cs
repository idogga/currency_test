using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonService.Commands.Login;
using PersonService.Commands.Logout;
using PersonService.Commands.Register;
using PersonService.Commands.ShowAllUsers;
using PersonService.Dtos;

namespace PersonService.Web;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    /// <summary>
    /// MediatR для обработки команд.
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// Маппер для преобразования моделей в DTO и обратно.
    /// </summary>
    private readonly IMapper _mapper;

    public UsersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Обрабатывает запрос на регистрацию нового пользователя.
    /// </summary>
    /// <param name="command">Команда для регистрации пользователя.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns></returns>
    [HttpPost()]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Обрабатывает запрос на получение списка всех пользователей.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список пользователей</returns>
    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _mediator.Send(new ShowAllUsersCommand(), cancellationToken);
        var userDtos = users.Select(_mapper.Map<UserDto>).ToList();
        return Ok(userDtos);
    }

    /// <summary>
    /// Обрабатывает запрос на вход пользователя в систему.
    /// </summary>
    /// <param name="command">Команда для входа пользователя.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Обрабатывает запрос на выход пользователя из системы.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var expClaim = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
        if (jti is null || expClaim is null)
            return BadRequest("Invalid token.");

        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;
        await _mediator.Send(new LogoutCommand(jti, expiresAt), cancellationToken);
        return NoContent();
    }
}