using System.Globalization;
using System.Xml.Serialization;
using AutoMapper;
using CurrencyScheduler.Common.Interfaces;
using CurrencyScheduler.Infrastructure.Persistence;
using CurrencyScheduler.Models;
using EFCore.BulkExtensions;

namespace CurrencyScheduler.Common.Services;

/// <summary>
/// Сервис для загрузки и сохранения курсов валют из XML_daily API ЦБ РФ.
/// </summary>
public class CbrXmlDailyService : ICbrDailyService
{
    /// <summary>
    /// Фабрика для создания HttpClient.
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Автоматический маппер для преобразования моделей в DTO.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<CbrXmlDailyService> _logger;

    /// <summary>
    /// Контекст базы данных.
    /// </summary>
    private readonly AppDbContext _db;

    public CbrXmlDailyService(
        AppDbContext db,
        IHttpClientFactory httpClientFactory,
        IMapper mapper,
        ILogger<CbrXmlDailyService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _mapper = mapper;
        _logger = logger;
        _db = db;
    }

    /// <inheritdoc />
    public async Task DownloadAndSaveAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Начало загрузки XML_daily с API ЦБ РФ...");
        var client = _httpClientFactory.CreateClient("CbrClient");

        var dateStr = DateTime.UtcNow.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        var url = $"scripts/XML_daily.asp?date_req={dateStr}";

        try
        {
            var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("HTTP-ответ получен. Статус: {StatusCode}", response.StatusCode);

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var xml = System.Text.Encoding.GetEncoding("windows-1251").GetString(bytes);
            _logger.LogDebug("Получен XML длиной {Length} символов.", xml.Length);

            var serializer = new XmlSerializer(typeof(ValCurs));
            using var reader = new StringReader(xml);
            var valCurs = (ValCurs)serializer.Deserialize(reader)!;

            _logger.LogInformation("XML успешно десериализован. Дата: {Date}, валют: {Count}",
                valCurs.Date, valCurs.Valutes.Count);

            var dtos = _mapper.Map<List<Currency>>(valCurs);
            _logger.LogDebug("Выполнен маппинг {Count} записей в DTO.", dtos.Count);
            await _db.BulkInsertOrUpdateAsync(dtos, new BulkConfig
            {
                UpdateByProperties =
                [
                    nameof(Currency.Name),
                ],
                BatchSize = 50,
                UseOptionLoopJoin = true,
                SetOutputIdentity = false
            }, cancellationToken: cancellationToken);

            _logger.LogDebug("Запись в базу данных завершена. Всего записей: {Count}.", dtos.Count);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка HTTP-запроса при загрузке XML_daily.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Непредвиденная ошибка при обработке XML_daily.");
            throw;
        }
    }
}