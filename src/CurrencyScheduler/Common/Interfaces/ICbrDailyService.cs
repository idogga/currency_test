namespace CurrencyScheduler.Common.Interfaces;

/// <summary>
/// Интерфейс для сервиса, который загружает и сохраняет курсы валют.
/// </summary>
public interface ICbrDailyService
{
    /// <summary>
    /// Получает курсы валют и сохраняет их.
    /// </summary>
    Task DownloadAndSaveAsync(CancellationToken cancellationToken = default);
}
