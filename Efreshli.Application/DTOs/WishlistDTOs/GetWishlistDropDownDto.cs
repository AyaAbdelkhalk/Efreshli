namespace Efreshli.Application.DTOs.WishlistDTOs
{
    public class GetWishlistDropDownDto
    {
        public int WishlistId { get; set; }
        public string WishlistName { get; set; }
        public int ItemsCount { get; set; }
        public bool IsInWishlist { get; set; } = false;
    }
}
