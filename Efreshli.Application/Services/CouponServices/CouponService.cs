using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Interfaces;
using Efreshli.Domain.Models;
using Mapster;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.CouponServices
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;

            //// Configure Mapster mappings if not configured globally
            //ConfigureMappings();
        }

        public async Task<CouponDTO> GetCouponByIdAsync(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            return coupon.Adapt<CouponDTO>();
        }

        public async Task<CouponDTO> GetCouponByCodeAsync(string code)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code);
            return coupon.Adapt<CouponDTO>();
        }

        public async Task<IEnumerable<CouponDTO>> GetAllCouponsAsync()
        {
            var coupons = await _couponRepository.GetAllAsync();
            return coupons.Adapt<IEnumerable<CouponDTO>>();
        }

        public async Task<CouponDTO> CreateCouponAsync(AddCouponDTO couponDto)
        {
            var coupon = couponDto.Adapt<Coupon>();
            coupon.IsActive = true;
            coupon.UsedCount = 0;

            await _couponRepository.AddAsync(coupon);
            await _couponRepository.SaveChangesAsync();
            return coupon.Adapt<CouponDTO>();
        }

        public async Task UpdateCouponAsync(UpdateCouponDTO couponDto)
        {
            var coupon = await _couponRepository.GetByIdAsync(couponDto.CouponId);
            if (coupon == null)
                throw new KeyNotFoundException("Coupon not found");

            couponDto.Adapt(coupon);
            await _couponRepository.UpdateAsync(coupon);
            await _couponRepository.SaveChangesAsync();
        }

        public async Task DeleteCouponAsync(int id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
                throw new KeyNotFoundException("Coupon not found");

            await _couponRepository.RemoveAsync(id);
            await _couponRepository.SaveChangesAsync();
        }

        public async Task<bool> ValidateCouponAsync(string code)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code);
            if (coupon == null || !coupon.IsActive)
                return false;

            return coupon.UsedCount < coupon.UsageLimit;
        }

        public async Task<CouponDTO> ApplyCouponAsync(string code)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code);
            if (coupon == null || !coupon.IsActive)
                throw new InvalidOperationException("Invalid coupon");

            if (coupon.UsedCount >= coupon.UsageLimit)
                throw new InvalidOperationException("Coupon usage limit reached");

            coupon.UsedCount++;
            await _couponRepository.UpdateAsync(coupon);
            await _couponRepository.SaveChangesAsync();

            return coupon.Adapt<CouponDTO>();
        }

        public async Task<bool> CouponCodeExistsAsync(string code)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code);
            return coupon != null;
        }
    }
}
    
