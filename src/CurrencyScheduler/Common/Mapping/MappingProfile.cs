using System.Globalization;
using AutoMapper;
using CurrencyScheduler.Models;

namespace CurrencyScheduler.Common.Mapping;

/// <summary>
/// Профиль для маппинга моделей.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ValCurs, List<Currency>>()
            .ConvertUsing((src, dest, context) =>
                src.Valutes.Select(v => new Currency
                {
                    Id = Guid.NewGuid(),
                    Name = v.Name!,
                    Rate = ParseCbrDecimal(v.VunitRate?.Replace(',', '.') ?? "0"),
                }).ToList());
    }

    /// <summary>
    /// Инициализация культуры для парсинга чисел в формате ЦБ РФ.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static decimal ParseCbrDecimal(string value)
    {
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0m;

            var trimmed = value.Trim();

            var normalized = trimmed.Replace(',', '.');
            if (decimal.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                return result;

            if (decimal.TryParse(trimmed, NumberStyles.Float, new CultureInfo("ru-RU"), out result))
                return result;

            if (decimal.TryParse(normalized, NumberStyles.Float | NumberStyles.AllowThousands,
                    CultureInfo.InvariantCulture, out result))
                return result;

            return 0m;
        }
    }
}