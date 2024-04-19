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

    public Task<IQueryable<AcademicProgram>> GetAcademicProgramsAsync()
    {
        var ret = unitOfWork.AcademicProgramRepository.GetAllAsync();
        return ret;
    }

    public Task PostAcademicProgramAsync(AcademicProgram academicProgram)
    {
        var ret = unitOfWork.AcademicProgramRepository.AddAsync(academicProgram);
        return ret;
    }
}