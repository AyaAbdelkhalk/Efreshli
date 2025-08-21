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
    public class UpdateCouponValidator : AbstractValidator<UpdateCouponDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public UpdateCouponValidator(IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = stringLocalizer;

            // Code - required + unique
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                .MustAsync(BeUniqueCode)
                .WithMessage(_localizer[SharedResourcesKeys.Validation.UniqueValueRequired]);

            // Discount Value - must be > 0
            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage(_localizer[SharedResourcesKeys.Validation.MustBeGreaterThanZero, 0]);

            // Usage Limit - must be >= 0 (or > 0 if you don't allow unlimited)
            RuleFor(x => x.UsageLimit)
                .GreaterThanOrEqualTo(0).WithMessage(_localizer[SharedResourcesKeys.Validation.MustBeGreaterThanZero, 0])
                .MustAsync(ValidUsageLimit);
            RuleFor(x => x.ExpireDate.Date)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage(_localizer[SharedResourcesKeys.Validation.FutureDateRequired]);
            RuleFor(x => x.MinOrderAmount)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                .GreaterThan(0).WithMessage(_localizer[SharedResourcesKeys.Validation.MustBeGreaterThanZero]);
            // If percentage, must be ≤ 100
            When(x => x.IsPercentage, () =>
            {
                RuleFor(x => x.DiscountValue)
                    .LessThanOrEqualTo(100)
                    .WithMessage(_localizer[SharedResourcesKeys.Validation.ValueMustBeLessThan, 100]);
            });
            
        }

        private async Task<bool> BeUniqueCode(UpdateCouponDTO dto, string code, CancellationToken cancellationToken)
        {
            var existingCoupons = await _unitOfWork.CouponRepository.GetWhereAsync(c => c.Code == code);
            var existingCoupon = existingCoupons.FirstOrDefault();
            return existingCoupon == null || existingCoupon.CouponId == dto.CouponId;
        }
        private async Task<bool> ValidUsageLimit(UpdateCouponDTO dto, int newUsageLimit, CancellationToken cancellationToken)
        {
            // Get only the UsedCount for the specific coupon
            var usedCount = (await _unitOfWork.CouponRepository
                .GetWhereSelectAsync(
                    c => c.CouponId == dto.CouponId,  // Condition: filter by coupon ID
                    c => c.UsedCount               // Selector: get only UsedCount
                    
                ))
                .FirstOrDefault();  // Get single result

            // Validate new limit is not less than current usage
            return newUsageLimit >= usedCount;
        }
    }

}