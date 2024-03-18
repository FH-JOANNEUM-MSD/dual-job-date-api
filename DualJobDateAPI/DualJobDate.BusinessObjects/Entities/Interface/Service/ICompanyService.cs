namespace DualJobDate.BusinessObjects.Entities.Interface.Service
{
    public interface ICompanyService
    {
        Task<string> GetCompanyByIdAsync(int id);
    }
}
