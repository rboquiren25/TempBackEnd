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
        public ICollection<Role> Roles {get; set;}

        public User()
        {
            Roles = new Collection<Role>();
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
}