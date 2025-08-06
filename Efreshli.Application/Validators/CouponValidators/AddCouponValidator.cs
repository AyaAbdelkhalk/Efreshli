using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Services.CouponServices;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.CouponValidators
{
    public class AddCouponValidator : AbstractValidator<AddCouponDTO>
    {
        private readonly ICouponService _couponService;

        public AddCouponValidator(ICouponService couponService)
        {
            _couponService = couponService;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("This code already exist.")
                .Length(4, 20).WithMessage("Coupon code must be between 4 and 20 characters")
                .MustAsync(async (code,CancellationToken) => { return !await _couponService.CouponCodeExistsAsync(code); }).WithMessage("Coupon code must be unique");
                //.MustAsync(BeUniqueCode).WithMessage("Coupon code must be unique");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage("Discount value must be greater than 0");

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0).WithMessage("Usage limit must be greater than 0");

            When(x => x.IsPercentage, () => {
                RuleFor(x => x.DiscountValue)
                    .LessThanOrEqualTo(100).WithMessage("Percentage discount cannot exceed 100%");
            });
        }

        //private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        //{
        //    return !await _couponService.CouponCodeExistsAsync(code);
        //}
    }
}