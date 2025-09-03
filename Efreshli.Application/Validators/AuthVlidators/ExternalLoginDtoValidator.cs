using Efreshli.Application.DTOs.IdentityDTOs;

using FluentValidation;


namespace Efreshli.Application.Validators.AuthVlidators
{
   
    public class ExternalLoginDtoValidator : AbstractValidator<ExternalLoginDto>
    {
        public ExternalLoginDtoValidator()
        {
            RuleFor(x => x.Provider)
                .NotEmpty()
                .Must(provider => provider == "Google" || provider == "Facebook")
                .WithMessage("Only Google and Facebook providers are supported.");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("A valid provider token is required.");
        }
    }
    }
