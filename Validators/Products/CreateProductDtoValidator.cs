using FluentValidation;
using WebApiAdvance.Entities.Dtos;

namespace WebApiAdvance.Validators.Products
{
	public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
	{
        public CreateProductDtoValidator()
        {
            RuleFor(p=>p.Name).NotEmpty().WithMessage("Name field must not be empty!").NotNull().WithMessage("Name field must not be null!")
                .MaximumLength(100)
                .MinimumLength(3)
                .Must(MustBeStartWithA).WithMessage("Product name must be start A");
            RuleFor(p=>p.Price).NotNull().GreaterThanOrEqualTo(100).LessThanOrEqualTo(10000);

        }
        public bool MustBeStartWithA(string name)
        {
            return name.StartsWith('A');
        }
    }
}
