using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Resources;
using Efreshli.Domain.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.AuthVlidators
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        private readonly IUserContext _userContext;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public ChangePasswordDtoValidator(IUserContext userContext, IStringLocalizer<SharedResources> localizer)
        {
            _userContext = userContext;
            _localizer = localizer;
            if (_userContext.HasPassword)
            {
                RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required]);

            }

            RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
            .MinimumLength(6).WithMessage(_localizer[SharedResourcesKeys.Validation.MinLength, 6])
            .Matches("[A-Z]").WithMessage(_localizer[SharedResourcesKeys.Validation.PasswordMissingUppercase])
            .Matches("[a-z]").WithMessage(_localizer[SharedResourcesKeys.Validation.PasswordMissingLowercase])
            .Matches("[0-9]").WithMessage(_localizer[SharedResourcesKeys.Validation.PasswordMissingDigit])
            .Matches("[^a-zA-Z0-9]").WithMessage(_localizer[SharedResourcesKeys.Validation.PasswordMissingSpecialChar]);
           
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword).WithMessage(_localizer[SharedResourcesKeys.Validation.PasswordsDoNotMatch]);
        }
    }
}
