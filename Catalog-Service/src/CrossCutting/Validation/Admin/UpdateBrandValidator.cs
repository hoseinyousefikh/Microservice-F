using Catalog_Service.src._03_Endpoints.DTOs.Requests.Admin;
using FluentValidation;

namespace Catalog_Service.src.CrossCutting.Validation.Admin
{
    public class UpdateBrandValidator : AbstractValidator<UpdateBrandRequest>
    {
        public UpdateBrandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid brand ID");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required")
                .Length(2, 100).WithMessage("Brand name must be between 2 and 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.LogoUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.LogoUrl))
                .WithMessage("Invalid logo URL format");

            RuleFor(x => x.MetaTitle)
                .MaximumLength(60).WithMessage("Meta title cannot exceed 60 characters");

            RuleFor(x => x.MetaDescription)
                .MaximumLength(160).WithMessage("Meta description cannot exceed 160 characters");
        }
    }
}
