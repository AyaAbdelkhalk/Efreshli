using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.DTOs.OrderDTOs;
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
        #region Addmin functions
        Task<CouponDTO> GetCouponByIdAsync(int id);
        Task<CouponDTO> GetCouponByCodeAsync(string code);
        //Task ApplayCoupon(string code,);
        Task<IEnumerable<CouponDTO>> GetAllCouponsAsync();
        Task<CouponDTO> CreateCouponAsync(AddCouponDTO couponDto);
        Task UpdateCouponAsync(UpdateCouponDTO couponDto);
        public Task<Response<bool>> DeleteCouponAsync(int id);//1
        Task<bool> ActivateCouponAsync(int id);
        Task<bool> DeactivateCouponAsync(int id);
        #endregion

        #region user functions
        //Task<Response<OrderPreviewDto>> ApplyCouponAsync(string code,string userId);
        Task<Response<CouponValidationResponseDto>> ValidateCouponAsync(string couponCode, string userId);

        #endregion


        Task<bool> CouponCodeExistsAsync(string code);
       
    }
    
}