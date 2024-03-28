using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Resources;
using DualJobDate.DataAccess.Repositories;

namespace DualJobDate.Api.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<UserResource, User>();
            CreateMap<CompanyDetailsResource, CompanyDetails>();
            CreateMap<ActivityResource, CompanyActivity>();
        }
    }
}