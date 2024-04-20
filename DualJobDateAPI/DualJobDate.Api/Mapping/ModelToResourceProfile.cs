using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;

namespace DualJobDate.Api.Mapping;

public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<CompanyDetails, CompanyDetailsDto>();
        CreateMap<Company, CompanyDto>();
        CreateMap<CompanyActivity, ActivityDto>();
        CreateMap<StudentCompany, StudentCompanyDto>();
        CreateMap<Institution, InstitutionDto>();
        CreateMap<AcademicProgram, AcademicProgramDto>();
    }
}