using System.Xml.Serialization;

namespace CurrencyScheduler.Models;

/// <summary>
/// Модель для десериализации XML-ответа от ЦБ РФ, содержащего курсы валют.
/// </summary>
[XmlRoot("ValCurs")]
public class ValCurs
{
    /// <summary>
    /// Дата, на которую предоставлены курсы валют. Формат: "dd.MM.yyyy".
    /// </summary>
    [XmlAttribute("Date")]
    public string? Date { get; set; }

    /// <summary>
    /// Название типа курсов валют.
    /// </summary>
    [XmlAttribute("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Список валют с их курсами.
    /// </summary>
    [XmlElement("Valute")]
    public List<Valute> Valutes { get; set; } = [];
}

/// <summary>
/// Модель для представления информации о конкретной валюте и её курсе.
/// </summary>
public class Valute
{
    /// <summary>
    /// Уникальный идентификатор валюты.
    /// </summary>
    [XmlAttribute("ID")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Код валюты в числовом формате.
    /// </summary>
    [XmlElement("NumCode")]
    public string? NumCode { get; set; }

    /// <summary>
    /// Код валюты в буквенном формате.
    /// </summary>
    [XmlElement("CharCode")]
    public string? CharCode { get; set; }

    /// <summary>
    /// Название валюты.
    /// </summary>
    [XmlElement("Nominal")]
    public int? Nominal { get; set; }

    /// <summary>
    /// Значение курса валюты в рублях.
    /// </summary>
    [XmlElement("Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Значение курса валюты в рублях.
    /// </summary>
    [XmlElement("Value")]
    public string Value { get; set; } = null!;

    /// <summary>
    /// Значение курса единицы валюты в рублях.
    /// </summary>
    [XmlElement("VunitRate")]
    public string? VunitRate { get; set; }
}
