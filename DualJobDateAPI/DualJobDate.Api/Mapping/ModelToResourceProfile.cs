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
        CreateMap<User, CompanyUserDto>();
        CreateMap<Company, UserCompanyDto>();
        CreateMap<Address, AddressDto>();
        CreateMap<Company, CompanyDto>()
            .ForMember(dest => dest.Activities, opt => opt.MapFrom(src => src.CompanyActivities.Select(ca => new ActivityDto {
                Id = ca.Activity.Id,
                Name = ca.Activity.Name,
                Value = ca.Value // Angenommen, Value ist ein Feld in CompanyActivity
            }).ToList()));
    }
}