using Efreshli.Application.DTOs.WishlistDTOs.WishlistItemDTOs;

namespace Efreshli.Application.DTOs.WishlistDTOs
{
    public class GetWishlistDetailsDto
    {
        public int WishlistId { get; set; }
        public string WishlistName { get; set; }
        public int ItemsCount { get; set; }
        public string? WishlistUrl { get; set; }
        public List<string> MainImages { get; set; } = new List<string>();
        public List<LocalizedGetWishlistItemDto> wishlistItemsDto { get; set; } = new List<LocalizedGetWishlistItemDto>();

    }
}
