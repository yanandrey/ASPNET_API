using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIRest_ASPNET5.Models.Base
{
    public class BaseEntity
    {
        [Required]
        [Column("id")]
        public long Id { get; set; }
    }
}