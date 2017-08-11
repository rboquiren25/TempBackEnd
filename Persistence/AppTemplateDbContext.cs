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

    }   

}