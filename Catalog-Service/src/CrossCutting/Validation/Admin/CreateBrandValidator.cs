using Catalog_Service.src._03_Endpoints.DTOs.Requests.Admin;
using FluentValidation;
using System;

namespace Catalog_Service.src.CrossCutting.Validation.Admin
{
    public class CreateBrandValidator : AbstractValidator<CreateBrandRequest>
    {
        public CreateBrandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required.")
                .Length(2, 100).WithMessage("Brand name must be between 2 and 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.LogoUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.LogoUrl))
                .WithMessage("Invalid logo URL format.");

            // *** این بخش برای اعتبارسنجی WebsiteUrl اضافه شده است ***
            RuleFor(x => x.WebsiteUrl)
                .Must(BeAValidUrl)
                .When(x => !string.IsNullOrEmpty(x.WebsiteUrl))
                .WithMessage("Please enter a valid website URL (e.g., https://example.com).");

            RuleFor(x => x.MetaTitle)
                .MaximumLength(200).WithMessage("Meta title cannot exceed 200 characters.");

            RuleFor(x => x.MetaDescription)
                .MaximumLength(500).WithMessage("Meta description cannot exceed 500 characters.");
        }

        // *** این متد کمکی برای بررسی URL اضافه شده است ***
        private bool BeAValidUrl(string url)
        {
            // اگر خالی باشد، متد When جلوی آن را می‌گیرد، پس اینجا نیازی به چک نیست
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}