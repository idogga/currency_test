using CurrencyScheduler.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CurrencyScheduler.Jobs;

/// <summary>
/// Задача Hangfire для загрузки и сохранения курсов валют с сайта ЦБ РФ.
/// </summary>
public class CbrJob
{
    /// <summary>
    /// Сервис для загрузки и сохранения курсов валют с сайта ЦБ РФ.
    /// </summary>
    private readonly ICbrDailyService _service;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<CbrJob> _logger;

    public CbrJob(ICbrDailyService service, ILogger<CbrJob> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Hangfire-задача CbrJob запущена в {Time}", DateTime.UtcNow);
        try
        {
            await _service.DownloadAndSaveAsync();
            _logger.LogInformation("Hangfire-задача CbrJob успешно завершена.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hangfire-задача CbrJob завершилась с ошибкой.");
            throw;
        }
    }
}