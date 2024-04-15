namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IUtilService
{
    Task<IQueryable<Institution>> GetInstitutionsAsync();

}