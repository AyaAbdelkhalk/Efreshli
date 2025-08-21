using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Resources;
using Efreshli.Application.Services.CouponServices;
using FluentValidation;
using Microsoft.Extensions.Localization;
namespace Efreshli.Application.Validators.CouponValidators
{
    public class AddCouponDTOValidator : AbstractValidator<AddCouponDTO>
    {
        private readonly ICouponService _couponService;
        private readonly IStringLocalizer<SharedResources> _localizer;
        public AddCouponDTOValidator(ICouponService couponService, IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
            _couponService = couponService;

            RuleFor(x => x.Code)
     .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
     .Length(4, 20).WithMessage(_localizer[SharedResourcesKeys.Validation.LengthRange, 4, 20])
     .MustAsync(async (code, cancellationToken) => !await _couponService.CouponCodeExistsAsync(code))
     .WithMessage(_localizer[SharedResourcesKeys.Validation.UniqueValueRequired]);


            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage(_localizer[SharedResourcesKeys.Validation.MustBeGreaterThanZero]);

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0).WithMessage(_localizer[SharedResourcesKeys.Validation.MustBeGreaterThanZero]);

            RuleFor(x => x.ExpireDate.Date)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage(_localizer[SharedResourcesKeys.Validation.FutureDateRequired]);
            RuleFor(x => x.MinOrderAmount)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                .GreaterThan(0).WithMessage(_localizer[SharedResourcesKeys.Validation.MustBeGreaterThanZero]);
            When(x => x.IsPercentage, () =>
            {
                RuleFor(x => x.DiscountValue)
                    .LessThanOrEqualTo(100).WithMessage(_localizer[SharedResourcesKeys.Validation.ValueMustBeLessThan, 100]);
            });
        }

        //private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        //{
        //    return !await _couponService.CouponCodeExistsAsync(code);
        //}
    }
}