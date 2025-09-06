using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Mapping
{
    public static class CouponMappingConfig
    {
        public static void Configure()
        {
            // Coupon -> CouponDTO
            TypeAdapterConfig<Coupon, CouponDTO>.NewConfig()
                .Map(dest => dest.CouponId, src => src.CouponId)
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.IsActive, src => src.IsActive)
                .Map(dest => dest.DiscountValue, src => src.DiscountValue)
                .Map(dest => dest.IsPercentage, src => src.IsPercentage)
                .Map(dest => dest.UsageLimit, src => src.UsageLimit)
                .Map(dest => dest.UsedCount, src => src.UsedCount);

            // AddCouponDTO -> Coupon
            TypeAdapterConfig<AddCouponDTO, Coupon>.NewConfig()
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.DiscountValue, src => src.DiscountValue)
                .Map(dest => dest.IsPercentage, src => src.IsPercentage)
                .Map(dest => dest.UsageLimit, src => src.UsageLimit)
                .Ignore(dest => dest.CouponId)
                .Ignore(dest => dest.IsActive)
                .Ignore(dest => dest.UsedCount)
                .AfterMapping((src, dest) =>
                {
                    dest.IsActive = true; // Default value
                    dest.UsedCount = 0;   // Default value
                });

            // UpdateCouponDTO -> Coupon
            TypeAdapterConfig<UpdateCouponDTO, Coupon>.NewConfig()
                .Map(dest => dest.CouponId, src => src.CouponId)
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.DiscountValue, src => src.DiscountValue)
                .Map(dest => dest.IsPercentage, src => src.IsPercentage)
                .Map(dest => dest.UsageLimit, src => src.UsageLimit)
                .Ignore(dest => dest.UsedCount); // Don't allow updating UsedCount through DTO

            
        }
    }
}
