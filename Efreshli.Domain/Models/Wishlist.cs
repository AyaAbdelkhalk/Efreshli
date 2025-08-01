using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public ICollection<WishlistItem> WishlistItems { get; set; }
    }
}
