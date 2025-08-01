using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efreshli.Domain.Models
{
    public class Wishlist : Auditable
    {
        [Key]
        public int WishlistId { get; set; }
        public string Name { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<WishlistItem>? WishlistItems { get; set; }
    }
}
