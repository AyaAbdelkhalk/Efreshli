using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.HomeServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public ShopController(IHomeService homeService)
        {
            _homeService = homeService;
        }
        [HttpGet("SearchProducts")]
        public async Task<IActionResult> SearchProducts(string keyword=null,int pageNumber = 1, int pageSize = 24)
        {
            var result = await _homeService.SearchProducts(keyword, pageNumber, pageSize);
            return this.CreateResponse(result);
        }
        [HttpGet("Products/{BrandId}")]
        public async Task<IActionResult> GetBrandProducts(int BrandId)
        {
            var result = await _homeService.GetBrandProducts(BrandId);
            return this.CreateResponse(result);
        }
    }
}
