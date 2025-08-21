using Efreshli.Application.DTOs.CartDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.CartValidators
{
    public class UpdateCartItemQuantityRequestDtoValidator : AbstractValidator<UpdateCartItemQuantityRequestDto>
    {
        public UpdateCartItemQuantityRequestDtoValidator()
        {
            RuleFor(x => x.CartItemId).GreaterThan(0).WithMessage("Cart Item ID must be greater than 0.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }   
}
