using AutoMapper;
using DualJobDate.API.Resources;
using DualJobDate.BusinessObjects.Entities;

namespace The_Reading_Muse_API.Mapping;

public class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        CreateMap<UserResource, User>();
    }
}