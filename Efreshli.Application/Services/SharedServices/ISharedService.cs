using Efreshli.Application.Helper.ResultPattern;

namespace Efreshli.Application.Services.SharedServices
{
    public interface ISharedService
    {
        Task<Response<bool>> IsItemWishlisted(int itemId);
        Task<List<int>> GetWishlistedProductIdsAsync(string userId);
        Task<Response<List<int>>> GetWishlistedProductIds(List<int> productIds);
    }
}
