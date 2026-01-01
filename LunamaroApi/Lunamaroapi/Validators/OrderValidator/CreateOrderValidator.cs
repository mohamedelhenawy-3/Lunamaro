using FluentValidation;
using Lunamaroapi.DTOs.Order;

namespace Lunamaroapi.Validators.OrderValidator
{
    public class CreateOrderValidator :AbstractValidator<CreateOrderdto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.TemporaryKey)
                 .NotEmpty().WithMessage("Temporary key is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .MaximumLength(20);

            RuleFor(x => x.DeliveryStreetAddress)
                .NotEmpty().WithMessage("Street address is required")
                .MaximumLength(200);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100);

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required")
                .MaximumLength(100);

            RuleFor(x => x.PostalCode)
                .GreaterThan(0).WithMessage("Postal code must be greater than 0");

        }


    }
}
