using Catalog_Service.src._03_Endpoints.DTOs.Requests.Public;
using FluentValidation;

namespace Catalog_Service.src.CrossCutting.Validation.Public
{
    public class CreateProductReviewValidator : AbstractValidator<CreateProductReviewRequest>
    {
        public CreateProductReviewValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Valid product ID is required");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Review title is required")
                .Length(5, 100).WithMessage("Title must be between 5 and 100 characters");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Review comment is required")
                .Length(20, 1000).WithMessage("Comment must be between 20 and 1000 characters");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
        }
    }
}
