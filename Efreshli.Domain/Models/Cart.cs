using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Cart : Auditable
    {
        [Key]
        public int CartId { get; set; }
        public int UserId { get; set; }
        public ICollection<CartItem> Items { get; set; }
    }
}
