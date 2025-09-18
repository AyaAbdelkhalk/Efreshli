using Efreshli.Application.DTOs.OrderDTOs;
using FluentValidation;

namespace Efreshli.Application.Validators.OrderValidators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.AddressId)
                .GreaterThan(0)
                .WithMessage("Address ID must be greater than 0");

            

            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithMessage("Please select a valid payment method");

            RuleFor(x => x.Note)
                .MaximumLength(500)
                .WithMessage("Note cannot exceed 500 characters");

            RuleFor(x => x.DeliveryNotes)
                .MaximumLength(500)
                .WithMessage("Delivery notes cannot exceed 500 characters");
        }
    }
}