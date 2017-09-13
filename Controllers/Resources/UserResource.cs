using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyTemplate.Controllers.Resources
{
    public class UserResource
    {
        
        public int Id { get; set; }        
        public string Username { get; set; }      
        public string Password { get; set; }       
        public string Email { get; set; }   
        public ICollection<RoleResource> Roles { get; set; }
        public ICollection<ScopeResource> Scopes { get; set; }
        
        public UserResource()
        {
            Roles = new Collection<RoleResource>();
            Scopes = new Collection<ScopeResource>();
        }
    }
}