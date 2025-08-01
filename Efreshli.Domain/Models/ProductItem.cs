using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class ProductItem : Auditable
    {
        [Key]
        public int ProductItemId { get; set; }
        public decimal Price { get; set; }

        public int? FabricColorId { get; set; }
        public Color? FabricColor { get; set; }

        public int? WoodColorId { get; set; }
        public Color? WoodColor { get; set; }

        public decimal Discount { get; set; }
        public bool IsPercentage { get; set; }
        public int Qty { get; set; }
        public string SKU { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
