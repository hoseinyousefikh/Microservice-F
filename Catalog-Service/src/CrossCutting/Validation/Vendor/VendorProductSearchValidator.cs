using Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor;
using FluentValidation;

namespace Catalog_Service.src.CrossCutting.Validation.Vendor
{
    public class VendorProductSearchValidator : AbstractValidator<VendorProductSearchRequest>
    {
        public VendorProductSearchValidator()
        {
            // می‌توانید قوانین اعتبارسنجی مورد نیاز خود را اینجا اضافه کنید
            // مثلاً برای PageNumber و PageSize

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal to 1.")
                .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");
        }
    }
}
