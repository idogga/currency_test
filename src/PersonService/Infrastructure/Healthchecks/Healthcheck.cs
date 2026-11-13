using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PersonService.Infrastructure.Healthchecks;

/// <summary>
/// Предоставляет проверку состояния приложения.
/// </summary>
public class HealthCheck : IHealthCheck
{
    /// <summary>
    /// Возвращает успешный результат проверки состояния приложения.
    /// </summary>
    /// <param name="context">Контекст выполнения проверки состояния.</param>
    /// <param name="cancellationToken">Токен отмены проверки.</param>
    /// <returns>Результат, указывающий на работоспособное состояние приложения.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}