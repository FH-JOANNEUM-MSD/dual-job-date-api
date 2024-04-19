using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Resources;

namespace DualJobDate.Api.Mapping;

public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<User, UserResource>();
        CreateMap<CompanyDetails, CompanyDetailsResource>();
        CreateMap<Company, CompanyResource>();
        CreateMap<CompanyActivity, ActivityResource>();
        CreateMap<Institution, InstitutionResource>();
        CreateMap<AcademicProgram, AcademicProgramResource>();
    }
}