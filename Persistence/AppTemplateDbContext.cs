using MyTemplate.Models;
using Microsoft.EntityFrameworkCore;


namespace MyTemplate.Persistence
{
    public class MyTemplateDbContext :DbContext
    {
        public MyTemplateDbContext(DbContextOptions<MyTemplateDbContext> options)
        :base(options)
        {
        }

        public DbSet<User> Users {get; set;}
        public DbSet<Role> Roles {get; set;}
        public DbSet<Location> Locations {get;set;}
        public DbSet<RoleType> RoleTypes {get;set;}
        public DbSet<Scope> Scopes {get; set;}

    }   

}