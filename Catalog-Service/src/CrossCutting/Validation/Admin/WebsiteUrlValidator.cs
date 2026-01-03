using FluentValidation;

namespace Catalog_Service.src.CrossCutting.Validation.Admin
{
    public class WebsiteUrlValidator : AbstractValidator<string>
    {
        public WebsiteUrlValidator()
        {
            RuleFor(url => url)
                .NotEmpty().WithMessage("Website URL cannot be empty.")
                .Must(BeAValidUrl).WithMessage("Please enter a valid website URL (e.g., https://example.com).");
        }

        private bool BeAValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return true; // یا false، بسته به اینکه آیا URL الزامی است یا خیر
            }

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
