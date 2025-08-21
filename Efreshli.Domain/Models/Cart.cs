using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efreshli.Domain.Models
{
    public class Cart : Auditable
    {
        [Key]
        public int CartId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<CartItem>? Items { get; set; } 
    }
}
