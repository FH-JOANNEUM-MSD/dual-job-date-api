using System.Text.Json.Serialization;

namespace DualJobDate.BusinessObjects.Dto;

public record CompanyDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name);
