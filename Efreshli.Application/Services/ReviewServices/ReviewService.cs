using Efreshli.Application.DTOs.Review;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.AIServices;
using Efreshli.Application.Services.File;
using Efreshli.Application.Services.ReviewServices;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ReviewServices
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly IAIService _aiService;
        private readonly IUserContext _userContext;

        public ReviewService(IUnitOfWork unitOfWork, IImageService imageService,IAIService aiService, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _aiService = aiService;
            _userContext = userContext;
        }

        public async Task<Response<ReviewResponseDto>> CreateReviewAsync(AddReviewDto model)
        {
            try
            {
                var review = new Review
                {
                    Rate = model.Rate,
                    ReviewText = model.ReviewText,
                    ProductId = model.ProductId,
                    Images = new List<Image>()
                };
                review.ApplicationUserId = _userContext.CurrentUserId??"";
                await _unitOfWork.ReviewRepository.AddAsync(review);
                await _unitOfWork.SaveChangesAsync();

                // Upload images
                if (model.Images != null && model.Images.Any())
                {
                    foreach (var file in model.Images)
                    {
                        var image = await _imageService.UploadImageAsync(file, ImageReferenceType.Review, review.Id);
                        review.Images.Add(image);
                    }

                    await _unitOfWork.SaveChangesAsync();
                }

                return ResponseHandler.Success(MapToResponseDto(review));
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<ReviewResponseDto>("Error creating review: " + ex.Message);
            }
        }

        public async Task<Response<ReviewResponseDto>> UpdateReviewAsync(UpdateReviewDto model)
        {
            try
            {
                var review = await _unitOfWork.ReviewRepository.GetByIdWithIncludeAsync(
                    model.Id,
                    includes: r => r.Images
                );

                if (review == null)
                    return ResponseHandler.NotFound<ReviewResponseDto>("Review not found");

                review.Rate = model.Rate;
                review.ReviewText = model.ReviewText;
                review.ProductId = model.ProductId;

                // Add new images
                if (model.Images != null && model.Images.Any())
                {
                    foreach (var file in model.Images)
                    {
                        var image = await _imageService.UploadImageAsync(file, ImageReferenceType.Review, review.Id);
                        review.Images.Add(image);
                    }
                }

                await _unitOfWork.ReviewRepository.UpdateAsync(review);
                await _unitOfWork.SaveChangesAsync();

                return ResponseHandler.Success(MapToResponseDto(review));
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<ReviewResponseDto>("Error updating review: " + ex.Message);
            }
        }

        public async Task<Response<bool>> DeleteReviewAsync(int id)
        {
            try
            {
                var review = await _unitOfWork.ReviewRepository.GetByIdWithIncludeAsync(
                    id,
                    includes: r => r.Images
                );

                if (review == null)
                    return ResponseHandler.NotFound<bool>("Review not found");

                // Delete related images
                if (review.Images != null && review.Images.Any())
                {
                    foreach (var img in review.Images.ToList())
                    {
                        await _imageService.DeleteImageAsync(img.Id);
                    }
                }

                await _unitOfWork.ReviewRepository.RemoveAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<bool>("Error deleting review: " + ex.Message);
            }
        }

        private ReviewResponseDto MapToResponseDto(Review review)
        {
            return new ReviewResponseDto
            {
                Id = review.Id,
                Rate = review.Rate,
                ReviewText = review.ReviewText,
                ProductId = review.ProductId,
                Images = review.Images
            };
        }

        public async Task<Response<List<ReviewResponseDto>>> GetProductReviewsAsync(int productId)
        {
          var entity=await _unitOfWork.ReviewRepository.GetAll().Where(x=> x.ProductId == productId).ToListAsync();
            if(entity == null) return ResponseHandler.NotFound<List<ReviewResponseDto>>("No Reviews for this product");
            var result =entity.Adapt<List<ReviewResponseDto>>();
            return ResponseHandler.Success(result); 

        }

        public async Task<Response<ReviewResponseDto>> GetReviewByIdAsync(int reviewId)
        {
            var entity =await _unitOfWork.ReviewRepository.GetByIdAsync(reviewId);
            if (entity == null) return ResponseHandler.NotFound<ReviewResponseDto>();
            var result =entity.Adapt<ReviewResponseDto>();
            return ResponseHandler.Success(result);
        }

        public async Task<Response<List<string>>> GetSummerizedProductReview(int productId)
        {
            var ReviewsText =await _unitOfWork.ReviewRepository.GetAll().Where(x=>x.ProductId==productId).Select(x => x.ReviewText).ToListAsync();
            if (!ReviewsText.Any()) return ResponseHandler.NotFound<List<string>>("No Revirews");
             var summryReview=await _aiService.SummarizeReviewsAsync(ReviewsText);
            return ResponseHandler.Success(summryReview);
            
        }
    }

}
