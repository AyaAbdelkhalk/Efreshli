using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Resources;
using Efreshli.Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;


namespace Efreshli.Application.Validators.AuthVlidators
{
    public class CreateAdminDtoValidator : AbstractValidator<CreateAdminDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CreateAdminDtoValidator(UserManager<ApplicationUser> userManager, IStringLocalizer<SharedResources> localizer)
        {
            _userManager = userManager;
            _localizer = localizer;
            RuleFor(x => x.FirstName)
                          .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                          .MaximumLength(50).WithMessage(_localizer[SharedResourcesKeys.Validation.MaxLength, 50]);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                .MaximumLength(50).WithMessage(_localizer[SharedResourcesKeys.Validation.MaxLength, 50]);
           

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                .EmailAddress().WithMessage(_localizer[SharedResourcesKeys.Validation.EmailInvalid])
                .MustAsync(async (email, cancellation) => !await UserExists(email)).WithMessage(_localizer[SharedResourcesKeys.Validation.EmailAlreadyRegistered]); ;

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                .MinimumLength(6).WithMessage(_localizer[SharedResourcesKeys.Validation.MinLength,6]);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage(_localizer[SharedResourcesKeys.Validation.PasswordsDoNotMatch]);


        }
        private async Task<bool> UserExists(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}