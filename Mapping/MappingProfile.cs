using MyTemplate.Controllers.Resources;
using MyTemplate.Models;
using AutoMapper;

namespace MyTemplate.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //domain to resoure
            CreateMap<User, UserResource>();
            CreateMap<Role, RoleResource>();

            //resource to domain
            CreateMap<UserResource, User>(); 
            CreateMap<RoleResource, Role>();
        }
    }
}