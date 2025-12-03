using Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor;
using FluentValidation;

namespace Catalog_Service.src.CrossCutting.Validation.Vendor
{
    public class UpdateProductVariantValidator : AbstractValidator<UpdateProductVariantRequest>
    {
        public UpdateProductVariantValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid variant ID");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Variant name is required")
                .Length(2, 100).WithMessage("Variant name must be between 2 and 100 characters");

            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required")
                .Length(2, 50).WithMessage("SKU must be between 2 and 50 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0");

            RuleFor(x => x.Dimensions)
                .NotNull().WithMessage("Dimensions are required")
                .SetValidator(new DimensionsRequestValidator());

            RuleFor(x => x.OriginalPrice)
                .GreaterThan(x => x.Price)
                .When(x => x.OriginalPrice.HasValue)
                .WithMessage("Original price must be greater than current price");
        }
    }
}
