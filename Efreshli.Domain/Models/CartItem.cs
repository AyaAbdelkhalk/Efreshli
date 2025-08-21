using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class CartItem : Auditable
    {
        [Key]
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public virtual Cart? Cart { get; set; }

        public int ProductItemId { get; set; }
        public virtual ProductItem? ProductItem { get; set; }

        public int RequiredQuantity { get; set; } = 1;
    }
}
