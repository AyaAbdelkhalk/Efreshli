using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Resources;
using Efreshli.Application.Services.CouponServices;
using Efreshli.Domain.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Efreshli.Application.Validators.CouponValidators
{
    public class AddCouponDTOValidator : AbstractValidator<AddCouponDTO>
    {
        private readonly ICouponService _couponService;
        private readonly IStringLocalizer<SharedResources> _localizer;
        public AddCouponValidator(ICouponService couponService, IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
            _couponService = couponService;


            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                //.NotEmpty().WithMessage("This code already exist.")
                .Length(4, 20).WithMessage("Coupon code must be between 4 and 20 characters")
                .MustAsync(async (code,CancellationToken) => { return !await _couponService.CouponCodeExistsAsync(code); }).WithMessage(_localizer[SharedResourcesKeys.Validation.UniqueValueRequired]);
                //.MustAsync(BeUniqueCode).WithMessage("Coupon code must be unique");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage(_localizer[SharedResourcesKeys.Validation.MustBeGreaterThanZero]);
                //.GreaterThan(0).WithMessage("Discount value must be greater than 0");

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0).WithMessage(_localizer[SharedResourcesKeys.Validation.MustBeGreaterThanZero]);
                //.GreaterThan(0).WithMessage("Usage limit must be greater than 0");

            When(x => x.IsPercentage, () =>
            {
                RuleFor(x => x.DiscountValue)
                    .LessThanOrEqualTo(100).WithMessage(_localizer[SharedResourcesKeys.Validation.ValueMustBeLessThan,100]);
            });
        }

        //private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        //{
        //    return !await _couponService.CouponCodeExistsAsync(code);
        //}
    }
}