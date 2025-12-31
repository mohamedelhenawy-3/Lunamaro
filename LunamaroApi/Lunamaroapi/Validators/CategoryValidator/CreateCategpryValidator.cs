using FluentValidation;
using Lunamaroapi.DTOs.Category;

namespace Lunamaroapi.Validators.CategoryValidator
{
    public class CreateCategpryValidator :AbstractValidator<CreateCategoryDTO>
    {



        public CreateCategpryValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        }
    }
}
