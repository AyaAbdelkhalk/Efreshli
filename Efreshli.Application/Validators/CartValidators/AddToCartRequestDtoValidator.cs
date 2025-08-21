using Efreshli.Application.DTOs.CartDTOs;
using Efreshli.Domain.Common.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.CartValidators
{
    public class AddToCartRequestDtoValidator : AbstractValidator<AddToCartRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork; 

        public AddToCartRequestDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.ProductItemId)
                .GreaterThan(0).WithMessage("Product Item ID must be greater than 0.")
                .MustAsync(async (productItemId, cancellation) => await ProductItemExists(productItemId))
                .WithMessage("Product Item does not exist.");

            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }

        private async Task<bool> ProductItemExists(int productItemId)
        {
            
            var productItem = await _unitOfWork.ProductRepository.GetByIdAsync(productItemId);
            return productItem != null;
        }
    }
}
