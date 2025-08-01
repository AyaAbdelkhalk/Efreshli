using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public int UserId { get; set; }
        public ICollection<CartItem> Items { get; set; }
    }
}
