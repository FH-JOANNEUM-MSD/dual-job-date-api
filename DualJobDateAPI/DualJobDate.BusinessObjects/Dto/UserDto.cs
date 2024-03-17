using System.Text.Json.Serialization;

namespace DualJobDate.BusinessObjects.Dto;

public record UserDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("surname")] string Surname);
