namespace DualJobDate.BusinessObjects.Entities.Base.Interface;

public interface IBaseInstitutionEntity : IBaseEntity
{
    public Institution? Institution { get; set; }
    public int InstitutionId { get; set; }
}