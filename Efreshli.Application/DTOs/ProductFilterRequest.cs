using Efreshli.Domain.Enums;

namespace Efreshli.Application.DTOs
{
    public class ProductFilterRequest
    {
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 24;
        public ProductSortBy SortBy { get; set; } = ProductSortBy.Recommended;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? CategoryId { get; set; }
        public List<int>? BrandIds { get; set; }
        public int? FabricColorId { get; set; }
        public int? WoodColorId { get; set; }
    }
}
