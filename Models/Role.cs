using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTemplate.Models
{
    [Table("RoleTypes")]
    public class RoleType
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(25)]
        public string Name { get; set;}
    }
}