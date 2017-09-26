using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTemplate.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [StringLength(255)]
        public string Email { get; set; }   
        public ICollection<Role> Roles { get; set; }
        public ICollection<Scope> Scopes { get; set; }
        public ICollection<LoginLog> LoginLogs { get; set;}

        public User()
        {
            Roles = new Collection<Role>();
            Scopes = new Collection<Scope>();
            LoginLogs = new Collection<LoginLog>();
        }
    }

    [Table("Roles")]
    public class Role
    {
        public int Id { get; set; }
        [Required]
        public string RoleName { get; set; }
        public User User { get; set; }
        public int UserId {get; set; }
    }

    [Table("Scopes")]
    public class Scope
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public User User { get; set; }
        public int UserId {get; set; }
    }

    [Table("LoginLogs")]
    public class LoginLog {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string IpAddress { get; set;}
        public int UserId {get; set; }
    }
}