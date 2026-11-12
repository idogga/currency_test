namespace FinanceService.Infrastructure.Auth;

/// <summary>
/// Настройки для конфигурации JWT.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Имя секции конфигурации для JWT.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// URL авторизации, который будет использоваться для проверки токенов.
    /// </summary>
    public required string Authority { get; init; }

    /// <summary>
    /// Идентификатор аудитории, для которой предназначен токен.
    /// </summary>
    public required string Audience { get; init; }

    /// <summary>
    /// Идентификатор издателя, который будет использоваться для проверки токенов.
    /// </summary>
    public required string Issuer { get; init; }
}