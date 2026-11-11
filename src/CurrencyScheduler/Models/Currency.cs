using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyScheduler.Models;

/// <summary>
/// Валюта.
/// </summary>
[Table("currency")]
public record Currency
{
    /// <summary>
    /// Идентификатор валюты.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Имя валюты.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Курс валюты к рублю.
    /// </summary>
    public decimal Rate { get; set; }
}