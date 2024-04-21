using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DualJobDate.BusinessLogic.Services;

public class UtilService(IUnitOfWork unitOfWork) : IUtilService
{
    public Task<IQueryable<Institution>> GetInstitutionsAsync()
    {
        var ret = unitOfWork.InstitutionRepository.GetAllAsync();
        return ret;
    }
    
    public Task<Institution> GetInstitutionByKeyNameAsync(string keyName)
    {
        var ret = unitOfWork.InstitutionRepository.GetByName(keyName);
        return ret;
    }

    public Task<IQueryable<AcademicProgram>> GetAcademicProgramsAsync()
    {
        var ret = unitOfWork.AcademicProgramRepository.GetAllAsync();
        return ret;
    }
    
    public Task<AcademicProgram> GetAcademicProgramByKeyNameAndYearAsync(string keyName, int year)
    {
        var ret = unitOfWork.AcademicProgramRepository.GetByNameAndYear(keyName, year);
        return ret;
    }

    public async Task<AcademicProgram?> PostAcademicProgramAsync(int id, AcademicProgramModel model)
    {
        var ac = await unitOfWork.AcademicProgramRepository.GetByNameAndYear(model.KeyName, model.Year);
        if (ac != null)
        {
            throw new ArgumentException("AcademicProgram with same KeyName and Year already exists!");
        }
        unitOfWork.BeginTransaction();
        var academicProgram = new AcademicProgram
        {
            InstitutionId = id,
            Year = model.Year,
            Name = model.Name,
            KeyName = model.KeyName,
            AcademicDegreeEnum = model.AcademicDegreeEnum
        };
        await unitOfWork.AcademicProgramRepository.AddAsync(academicProgram);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
        return academicProgram;
    }
    
    public async Task<Institution?> PostInstitutionAsync(InstitutionModel model)
    {
        var i = await unitOfWork.InstitutionRepository.GetByName(model.KeyName);
        if (i != null)
        {
            throw new ArgumentException("Institution already exists!");
        }
        unitOfWork.BeginTransaction();
        var institution = new Institution
        {
            Name = model.Name,
            KeyName = model.KeyName,
            Website = model.Website
        };
        await unitOfWork.InstitutionRepository.AddAsync(institution);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
        return institution;
    }
}