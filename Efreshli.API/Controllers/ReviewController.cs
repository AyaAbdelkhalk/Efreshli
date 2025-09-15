using Efreshli.Application.DTOs.Review;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.ReviewServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService) {
            _reviewService = reviewService;
        }

        [HttpPost("reviews")]
        public async Task<IActionResult> AddReview(AddReviewDto request)
        {
            var result =await _reviewService.CreateReviewAsync(request);
            return this.CreateResponse(result);
        }

        [HttpGet("prouduct/{productId:int}/reviews/summry")]
        public async Task<IActionResult> GetReviewsSummry(int productId)
        {
            var result =await _reviewService.GetSummerizedProductReview(productId);
            return this.CreateResponse(result);
        }


        [HttpGet("reviews/{ProudcuId}")]
        public async Task<IActionResult> GetAllProductReviews(int ProudcuId)
        {
            var result = await _reviewService.GetProductReviewsAsync(ProudcuId);
            return this.CreateResponse(result);
        }
        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var result = await _reviewService.GetReviewByIdAsync(id);
            return this.CreateResponse(result);
        }
        [Authorize]
        [HttpDelete("{id}/reviews")]
        public async Task<IActionResult>DeleteReviewById(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            return this.CreateResponse(result);
        }


    }
}
