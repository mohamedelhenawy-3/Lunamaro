
using FluentValidation;
using Lunamaroapi.DTOs.Item;


namespace Lunamaroapi.Validators.ItemValidators
{
    public class ItemDTOValidator : AbstractValidator<ItemDTO>
    {
        public ItemDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(15);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.quantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CategoryId).GreaterThan(0);


            RuleSet("Create", () =>
            {
                RuleFor(x => x.File).NotNull().WithMessage("Image Is Required for Creteing Item");
            });
            RuleSet("Update", () =>
            {

            });
        }
    }
}
