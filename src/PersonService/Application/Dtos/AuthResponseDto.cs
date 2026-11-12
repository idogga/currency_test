namespace PersonService.Application.Dtos;

/// <summary>
/// DTO для передачи информации о пользователе после аутентификации.
/// </summary>
/// <param name="Token">JWT-токен.</param>
/// <param name="ExpiresAt">Время истечения токена.</param>
public record AuthResponseDto(string Token, DateTime ExpiresAt);
