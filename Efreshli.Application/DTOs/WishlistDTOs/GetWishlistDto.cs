using Efreshli.Application.DTOs.WishlistDTOs.WishlistItemDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.WishlistDTOs
{
    public class GetWishlistDto
    {
        public int WishlistId { get; set; }
        public string WishlistName { get; set; }
        public int ItemsCount { get; set; }
        public string? WishlistUrl { get; set; }
        public List<string> MainImages { get; set; } = new List<string>(); 
        public List<GetWishlistItemDto> wishlistItemsDto { get; set; } = new List<GetWishlistItemDto>();


    }
}
