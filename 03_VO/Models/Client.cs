using System.ComponentModel.DataAnnotations;
using APIRest_ASPNET5.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIRest_ASPNET5.Models
{
    [Table("client")]
    public class Client : BaseEntity
    {        
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column("cnh", TypeName = "varchar(50)")]
        public string CNH { get; set; }

        [Required]
        [Column("email", TypeName = "varchar(50)")]
        public string Email { get; set; }

        [Required]
        [Column("gender", TypeName = "varchar(10)")]
        public string Gender { get; set; }

        [Required]
        [Column("enabled")]
        public bool Enabled { get; set; }
    }
}