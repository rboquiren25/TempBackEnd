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
            CreateMap<Location, LocationResource>();
            CreateMap<RoleType, RoleListResource>();
            CreateMap<Scope, ScopeResource>();

            //resource to domain
            CreateMap<UserResource, User>(); 
            CreateMap<RoleResource, Role>();
            CreateMap<LocationResource, Location>();
            CreateMap<RoleListResource, RoleType>();
            CreateMap<ScopeResource, Scope>();

        }
    }
}