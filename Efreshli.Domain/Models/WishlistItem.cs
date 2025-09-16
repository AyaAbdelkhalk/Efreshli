using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class WishlistItem : Auditable
    {
        [Key]
        public int WishlistItemId { get; set; }
        public int WishlistId { get; set; }
        public  virtual Wishlist? Wishlist { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
