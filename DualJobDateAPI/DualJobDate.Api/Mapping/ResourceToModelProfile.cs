using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Resources;

namespace DualJobDate.Api.Mapping;

public class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        CreateMap<UserResource, User>();
        CreateMap<CompanyDetailsResource, CompanyDetails>();
        CreateMap<ActivityResource, CompanyActivity>();
        CreateMap<InstitutionResource, Institution>();
        CreateMap<AcademicProgramResource, AcademicProgram>();
    }
}