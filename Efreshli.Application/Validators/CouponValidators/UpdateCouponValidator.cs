using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Services.CouponServices;
using Efreshli.Domain.Common.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.CouponValidators
{
    public class UpdateCouponValidator : AbstractValidator<UpdateCouponDTO>
    {
        //private readonly ICouponService _couponService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCouponValidator(/*ICouponService couponService, */IUnitOfWork unitOfWork)
        {
            ///_couponService = couponService;

            RuleFor(x => x.CouponId)
                .GreaterThan(0).WithMessage("Invalid coupon ID");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Coupon code is required")
                .Length(4, 20).WithMessage("Coupon code must be between 4 and 20 characters")
                .MustAsync(BeUniqueCode).WithMessage("Coupon code must be unique");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage("Discount value must be greater than 0");

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0).WithMessage("Usage limit must be greater than 0")
                .GreaterThanOrEqualTo(x => x.UsedCount).WithMessage("Usage limit cannot be less than used count");

            When(x => x.IsPercentage, () =>
            {
                RuleFor(x => x.DiscountValue)
                    .LessThanOrEqualTo(100).WithMessage("Percentage discount cannot exceed 100%");
            });
            _unitOfWork = unitOfWork;
        }

        private async Task<bool> BeUniqueCode(UpdateCouponDTO dto, string code, CancellationToken cancellationToken)
        {
            var existingCoupons = await _unitOfWork.CouponRepository.GetWhereAsync(c=>c.Code==code);
            var existingCoupon = existingCoupons.FirstOrDefault();
            return existingCoupon == null || existingCoupon.CouponId == dto.CouponId;
        }
    }
}