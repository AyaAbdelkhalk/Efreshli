using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductItemId { get; set; }
        public ProductItem ProductItem { get; set; }

        public int RequiredQnty { get; set; }
    }
}
