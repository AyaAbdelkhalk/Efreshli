using Efreshli.Application.DTOs.Review;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ReviewServices
{
    public interface IReviewService
    {
        Task<Response<ReviewResponseDto>> CreateReviewAsync(AddReviewDto model);
        Task<Response<ReviewResponseDto>> UpdateReviewAsync(UpdateReviewDto model);
        Task<Response<bool>> DeleteReviewAsync(int id);

        Task<Response<List<ReviewResponseDto>>> GetProductReviewsAsync(int productId);
        Task<Response<ReviewResponseDto>> GetReviewByIdAsync(int reviewId);
        Task<Response<List<string>>> GetSummerizedProductReview(int productId);

    }
}
