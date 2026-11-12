namespace FinanceService.Application.Dtos;

/// <summary>
/// DTO для передачи информации о валюте.
/// </summary>
/// <param name="Id">Идентификатор валюты.</param>
/// <param name="Name">Название валюты.</param>
/// <param name="Rate">Значение курса единицы валюты в рублях.</param>
public sealed record CurrencyDto(Guid Id, string Name, decimal Rate);