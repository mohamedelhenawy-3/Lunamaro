using FluentValidation;
using Lunamaroapi.DTOs.Item;

namespace Lunamaroapi.Validators.ItemValidators
{
    public class UpdateItemDTOValidator :AbstractValidator<UpdateItemDTO>
    {


        public UpdateItemDTOValidator()
        {
            RuleFor(x => x.Name)
             .NotEmpty()
             .MaximumLength(100);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.Price)
                .GreaterThan(0);

            RuleFor(x => x.quantity)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0);
        }
    }
}
