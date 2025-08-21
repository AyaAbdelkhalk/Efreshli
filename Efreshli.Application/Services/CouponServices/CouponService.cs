using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.DTOs.OrderDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Interfaces;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using FluentValidation;
using Mapster;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Efreshli.Application.Resources.SharedResourcesKeys.Efreshli;
using Coupon = Efreshli.Domain.Models.Coupon;

namespace Efreshli.Application.Services.CouponServices
{
    public class CouponService : ICouponService
    {
        #region Props&Ctor
        private readonly IUnitOfWork _unitOfWork;
        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        #endregion

        public async Task<CouponDTO> GetCouponByIdAsync(int id)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id);
            return coupon.Adapt<CouponDTO>();
        }

        public async Task<CouponDTO> GetCouponByCodeAsync(string code)
        {
            var coupon = await _unitOfWork.CouponRepository.GetWhereAsync(c => c.Code == code);
            return coupon.Adapt<CouponDTO>();
        }

        public async Task<IEnumerable<CouponDTO>> GetAllCouponsAsync()
        {
            var coupons = await _unitOfWork.CouponRepository.GetAllAsync();
            return coupons.Adapt<IEnumerable<CouponDTO>>();
        }

        public async Task<CouponDTO> CreateCouponAsync(AddCouponDTO couponDto)
        {
            var coupon = couponDto.Adapt<Coupon>();
            coupon.IsActive = true;
            coupon.UsedCount = 0;

            await _unitOfWork.CouponRepository.AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync();
            return coupon.Adapt<CouponDTO>();
        }

        public async Task UpdateCouponAsync(UpdateCouponDTO couponDto)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(couponDto.CouponId);
            if (coupon == null)
                throw new KeyNotFoundException("Coupon not found");

            couponDto.Adapt(coupon);
            await _unitOfWork.CouponRepository.UpdateAsync(coupon);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Response<bool>> DeleteCouponAsync(int id)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id);
            if (coupon == null)
                return ResponseHandler.NotFound<bool>("Coupon not found");

            await _unitOfWork.CouponRepository.RemoveAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Success(true);
        }

        public async Task<bool> ValidateCouponAsync(string code)
        {
            var coupons = await _unitOfWork.CouponRepository.GetWhereAsync(c => c.Code == code);
            var coupon = coupons.FirstOrDefault();
            if (coupon == null || !coupon.IsActive)
                return false;

            return coupon.UsedCount < coupon.UsageLimit;
        }

        public async Task<CouponDTO> ApplyCouponAsync(string code)
        {
            var coupons = await _unitOfWork.CouponRepository.GetWhereAsync(c => c.Code == code);
            var coupon = coupons.FirstOrDefault();
            if (coupon == null || !coupon.IsActive)
                throw new InvalidOperationException("Invalid coupon");

            if (coupon.UsedCount >= coupon.UsageLimit)
                throw new InvalidOperationException("Coupon usage limit reached");

            coupon.UsedCount++;
            await _unitOfWork.CouponRepository.UpdateAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            return coupon.Adapt<CouponDTO>();
        }

        public async Task<bool> CouponCodeExistsAsync(string code)
        {
            var coupons = await _unitOfWork.CouponRepository.GetWhereAsync(c => c.Code == code);
            var coupon = coupons.FirstOrDefault();
            return coupon != null;
        }

        public async Task<bool> ActivateCouponAsync(int id)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                throw new KeyNotFoundException();
            }
            coupon.IsActive = true;
            await _unitOfWork.CouponRepository.UpdateAsync(coupon);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateCouponAsync(int id)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                throw new KeyNotFoundException();
            }
            coupon.IsActive = false;
            await _unitOfWork.CouponRepository.UpdateAsync(coupon);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Response<CouponValidationResponseDto>> ValidateCouponAsync(
              string couponCode, string userId)
        {
           
                // Input validation
                if (string.IsNullOrWhiteSpace(couponCode))
                    return ResponseHandler.ValidationError<CouponValidationResponseDto>("Coupon code is required");

                if (string.IsNullOrWhiteSpace(userId))
                    return ResponseHandler.ValidationError<CouponValidationResponseDto>("User ID is required");

            var subTotalPrice = 8998;//_unitOfWork.CartRepository;
                if (subTotalPrice <= 0)
                    return ResponseHandler.ValidationError<CouponValidationResponseDto>("Subtotal must be greater than zero");

                // Get coupon
                var coupon = await _unitOfWork.CouponRepository.GetFirstOrDefaultAsync(c => c.Code == couponCode);
                if (coupon == null)
                    return ResponseHandler.NotFound<CouponValidationResponseDto>("Coupon not found");

                // Validate coupon status
                if (!coupon.IsActive)
                    return ResponseHandler.BadRequest<CouponValidationResponseDto>("Coupon is not active");

                if (coupon.ExpireDate < DateTime.UtcNow)
                    return ResponseHandler.BadRequest<CouponValidationResponseDto>("Coupon has expired");

                if (coupon.UsedCount >= coupon.UsageLimit)
                    return ResponseHandler.BadRequest<CouponValidationResponseDto>("Coupon usage limit reached");

                // Check if user has already used this coupon
                var hasUsed = await HasUserUsedCouponAsync(coupon.CouponId, userId);
                if (hasUsed)
                    return ResponseHandler.BadRequest<CouponValidationResponseDto>("You have already used this coupon");

                // Check minimum order amount if specified
                if (coupon.MinOrderAmount.HasValue && subTotalPrice < coupon.MinOrderAmount.Value)
                    return ResponseHandler.BadRequest<CouponValidationResponseDto>(
                        $"Minimum order amount of {coupon.MinOrderAmount.Value} required for this coupon");

                // Calculate order preview
                var orderPreview = await CalculateOrderPreview(coupon, subTotalPrice);

                // Prepare success response
                var responseDto = new CouponValidationResponseDto
                {
                    IsValid = true,
                    Message = "Coupon is valid",
                    OrderPreview = orderPreview
                };

                return ResponseHandler.Success(responseDto, "Coupon validated successfully");
           
        }

        private async Task<OrderCheckOutPreviewDto> CalculateOrderPreview(Coupon coupon, decimal subTotalPrice)
        {
            decimal discountValue = coupon.IsPercentage
                ? Math.Min(subTotalPrice * (coupon.DiscountValue / 100), subTotalPrice)
                : Math.Min(coupon.DiscountValue, subTotalPrice);

            //var shippingPrice = await _shippingCalculator.CalculateShippingAsync(subTotalPrice - discountValue);
            var estimatedDeliveryDate =DateTime.UtcNow.AddDays(5);

            return new OrderCheckOutPreviewDto
            {
                SubTotalPrice = subTotalPrice,
                DiscountValue = discountValue,
                //ShippingPrice = shippingPrice,
                TotalPrice = subTotalPrice - discountValue,// + shippingPrice,
                EstimatedDeliveryDate = estimatedDeliveryDate
            };
        }

        public async Task<bool> HasUserUsedCouponAsync(int couponId, string userId)
        {
            var users = await _unitOfWork.UserRepository.GetAllWithIncludeAsync(
         predicate: u => u.Id == userId,
         includes: u => u.Coupons
          );

            var user = users.FirstOrDefault();

            return user?.Coupons?.Any(c => c.CouponId == couponId) ?? false;
        }
    }
}

