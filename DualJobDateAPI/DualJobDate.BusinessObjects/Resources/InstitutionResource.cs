namespace DualJobDate.BusinessObjects.Resources;

public class InstitutionResource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public string? Website { get; set; }
}