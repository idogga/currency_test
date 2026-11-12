namespace FinanceService.Domain.Entities;

/// <summary>
/// Валюта.
/// </summary>
public sealed class Currency
{
    /// <summary>
    /// Уникальный идентификатор валюты.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название валюты.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Значение курса единицы валюты в рублях. 
    /// </summary>
    public decimal Rate { get; set; }
}