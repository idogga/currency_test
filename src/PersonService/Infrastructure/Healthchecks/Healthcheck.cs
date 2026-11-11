using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PersonService.Infrastructure.Healthchecks;

/// <summary>
/// Предоставляет проверку состояния приложения.
/// </summary>
public class HealthCheck : IHealthCheck
{
    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}