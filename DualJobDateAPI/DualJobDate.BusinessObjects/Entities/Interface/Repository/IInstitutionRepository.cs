namespace DualJobDate.BusinessObjects.Entities.Interface.Repository
{
    public interface IInstitutionRepository : IBaseRepository<Institution>
    {
        Task<Institution> GetByName(string KeyName);
    }
}
