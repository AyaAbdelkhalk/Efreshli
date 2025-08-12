using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.CouponServices
{
    public interface ICouponService
    {
        Task<CouponDTO> GetCouponByIdAsync(int id);
        Task<CouponDTO> GetCouponByCodeAsync(string code);
        Task<IEnumerable<CouponDTO>> GetAllCouponsAsync();
        Task<CouponDTO> CreateCouponAsync(AddCouponDTO couponDto);
        Task UpdateCouponAsync(UpdateCouponDTO couponDto);
        public Task<Response<bool>> DeleteCouponAsync(int id);//1
        Task<bool> ValidateCouponAsync(string code);
        Task<CouponDTO> ApplyCouponAsync(string code);
        Task<bool> CouponCodeExistsAsync(string code);
        Task<bool> ActivateCouponAsync(int id);
        Task<bool> DeactivateCouponAsync(int id);
    }
}