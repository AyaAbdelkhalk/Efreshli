using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Resources;
using Efreshli.Domain.Common.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.AuthVlidators
{
    public class UpdateProfileUpdateValidator:AbstractValidator<UpdateProfileDto>
    {
        IStringLocalizer<SharedResources> _localizer;
        public UpdateProfileUpdateValidator(IStringLocalizer<SharedResources> localizer)
        {
            _localizer=localizer;

            RuleFor(x=> x.FirstName)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                .MaximumLength(50).WithMessage(_localizer[SharedResourcesKeys.Validation.MaxLength,50]);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
                .MaximumLength(50).WithMessage(_localizer[SharedResourcesKeys.Validation.MaxLength, 50]);
            RuleFor(x => x.PhoneNumber)
               .NotEmpty().WithMessage(_localizer[SharedResourcesKeys.Validation.Required])
               .Matches(@"^\+?[0-9]{7,15}$").WithMessage(_localizer[SharedResourcesKeys.Validation.PhoneNumberInvalid]);
        }

    }
    }
