namespace DualJobDate.BusinessObjects.Dtos;

public class InstitutionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public string? Website { get; set; }
}