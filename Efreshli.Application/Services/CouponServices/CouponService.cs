using Efreshli.Application.DTOs.CouponDTOs;
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
            var coupon = await _unitOfWork.CouponRepository.GetWhereAsync(c=>c.Code==code);
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
    }
}
    
