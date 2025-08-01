using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class WishlistItem : Auditable
    {
        [Key]
        public int Id { get; set; }
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }

        public int ProductItemId { get; set; }
        public ProductItem ProductItem { get; set; }
    }
}
