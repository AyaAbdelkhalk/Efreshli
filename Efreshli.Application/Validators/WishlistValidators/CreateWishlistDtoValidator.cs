using Efreshli.Application.DTOs.WishlistDTOs;
using Efreshli.Domain.Common.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.WishlistValidators
{
    public class CreateWishlistDtoValidator : AbstractValidator<CreateWishlistDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public CreateWishlistDtoValidator(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Wishlist name is required.")
                .MaximumLength(500).WithMessage("Wishlist name must not exceed 50 characters.")
                .MustAsync(async (name, cancellation) => await IsUniquName(name)).WithMessage("Wishlist name must be unique.");

        }
        private async Task<bool> IsUniquName(string newname)
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var res =await _unitOfWork.WishlistRepository.GetAllAsync(
                i => i.Name == newname && i.ApplicationUserId==userId
                );
            if(res.Count()>0)
                return false;
            else return true;
        }
    }
}
