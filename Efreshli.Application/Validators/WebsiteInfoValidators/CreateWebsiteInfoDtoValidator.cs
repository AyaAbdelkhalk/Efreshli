using Efreshli.Application.DTOs.WebsiteInfoDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.WebsiteInfoValidators
{
    public class CreateWebsiteInfoDtoValidator : AbstractValidator<CreateWebsiteInfoDto>
    {
        public CreateWebsiteInfoDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Please enter a valid email address");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required")
                .Matches(@"^[\+]?[0-9][\d]{0,15}$")
                .WithMessage("Please enter a valid phone number");

            RuleFor(x => x.DescriptionAr)
                .NotEmpty()
                .WithMessage("Arabic description is required")
                .MaximumLength(2000)
                .WithMessage("Arabic description cannot exceed 2000 characters");

            RuleFor(x => x.DescriptionEn)
                .NotEmpty()
                .WithMessage("English description is required")
                .MaximumLength(2000)
                .WithMessage("English description cannot exceed 2000 characters");

            RuleFor(x => x.Office)
                .MaximumLength(500)
                .WithMessage("Office information cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Office));

            RuleFor(x => x.Location)
                .MaximumLength(500)
                .WithMessage("Location cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Location));

            RuleFor(x => x.LogoURL)
                .Must(BeAValidUrl)
                .WithMessage("Please enter a valid URL for the logo")
                .When(x => !string.IsNullOrEmpty(x.LogoURL));

            RuleFor(x => x.FacebookLink)
                .Must(BeAValidUrl)
                .WithMessage("Please enter a valid Facebook URL")
                .When(x => !string.IsNullOrEmpty(x.FacebookLink));

            RuleFor(x => x.InstagramLink)
                .Must(BeAValidUrl)
                .WithMessage("Please enter a valid Instagram URL")
                .When(x => !string.IsNullOrEmpty(x.InstagramLink));

            RuleFor(x => x.YoutubeLink)
                .Must(BeAValidUrl)
                .WithMessage("Please enter a valid YouTube URL")
                .When(x => !string.IsNullOrEmpty(x.YoutubeLink));

            RuleFor(x => x.LinkedinLink)
                .Must(BeAValidUrl)
                .WithMessage("Please enter a valid LinkedIn URL")
                .When(x => !string.IsNullOrEmpty(x.LinkedinLink));
        }

        private static bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
