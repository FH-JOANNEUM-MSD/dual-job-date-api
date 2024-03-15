using AutoMapper;
using DualJobDate.API.Resources;
using DualJobDate.BusinessObjects.Entities;

namespace The_Reading_Muse_API.Mapping;

public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<User, UserResource>();
    }
}