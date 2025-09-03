using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;


namespace Efreshli.Application.Validators.AuthVlidators
{
    public class CreateAdminDtoValidator : AbstractValidator<CreateAdminDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateAdminDtoValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;


            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MustAsync(async (email, cancellation) => !await UserExists(email)).WithMessage("This email already used."); ;

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match");
           

        }
        private async Task<bool> UserExists(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}