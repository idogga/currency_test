namespace PersonService.Dtos;

/// <summary>
/// DTO для передачи информации о пользователе.
/// </summary>
/// <param name="Id">Идентификатор пользователя.</param>
/// <param name="Name">Имя пользователя.</param>
public record UserDto(Guid Id, string Name);