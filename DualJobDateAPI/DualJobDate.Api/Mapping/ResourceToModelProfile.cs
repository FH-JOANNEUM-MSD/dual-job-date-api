using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;

namespace DualJobDate.Api.Mapping;

public class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        CreateMap<UserDto, User>();
        CreateMap<CompanyDetailsDto, CompanyDetails>();
        CreateMap<ActivityDto, CompanyActivity>();
        CreateMap<InstitutionDto, Institution>();
        CreateMap<AcademicProgramDto, AcademicProgram>();
        CreateMap<AddressDto, Address>();
    }
}