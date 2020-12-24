using System.ComponentModel.DataAnnotations;
using APIRest_ASPNET5.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIRest_ASPNET5.Models
{
    [Table("vehicle")]
    public class Vehicle : BaseEntity
    {
        [Required]
        [Column("model", TypeName = "varchar(50)")]
        public string Model { get; set; }

        [Required]
        [Column("theyear")]
        public long Year { get; set; }

        [Required]
        [Column("plate", TypeName = "varchar(50)")]
        public string Plate { get; set; }

        [Required]
        [Column("color", TypeName = "varchar(50)")]
        public string Color { get; set; }

        [Required]
        [Column("enabled")]
        public bool Enabled { get; set; }
    }
}