using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;

namespace DualJobDate.BusinessLogic.Services;

public class UtilService(IUnitOfWork unitOfWork) : IUtilService
{
    public Task<IQueryable<Institution>> GetInstitutionsAsync()
    {
        var ret = unitOfWork.InstitutionRepository.GetAllAsync();
        return ret;
    }


}