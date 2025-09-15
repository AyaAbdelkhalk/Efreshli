namespace Efreshli.Application.DTOs.ProductDTOs
{
    public class LocalizedProductInfoDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal? FinalPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
}
