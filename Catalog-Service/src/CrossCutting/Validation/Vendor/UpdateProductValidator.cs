using Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor;
using FluentValidation;

namespace Catalog_Service.src.CrossCutting.Validation.Vendor
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid product ID");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .Length(2, 200).WithMessage("Product name must be between 2 and 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product description is required")
                .Length(10, 2000).WithMessage("Description must be between 10 and 2000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required")
                .Length(2, 50).WithMessage("SKU must be between 2 and 50 characters");

            RuleFor(x => x.BrandId)
                .GreaterThan(0).WithMessage("Valid brand ID is required");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Valid category ID is required");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0");

            RuleFor(x => x.Dimensions)
                .NotNull().WithMessage("Dimensions are required")
                .SetValidator(new DimensionsRequestValidator());

            RuleFor(x => x.OriginalPrice)
                .GreaterThan(x => x.Price)
                .When(x => x.OriginalPrice.HasValue)
                .WithMessage("Original price must be greater than current price");

            RuleFor(x => x.MetaTitle)
                .MaximumLength(60).WithMessage("Meta title cannot exceed 60 characters");

            RuleFor(x => x.MetaDescription)
                .MaximumLength(160).WithMessage("Meta description cannot exceed 160 characters");
        }
    }
}
