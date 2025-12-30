using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Catalog_Service.src.CrossCutting.Validation
{
    public class CustomValidationFilter : IAsyncActionFilter
    {
        private readonly IValidatorFactory _validatorFactory;

        public CustomValidationFilter(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var arguments = context.ActionArguments.Values;

            foreach (var arg in arguments)
            {
                if (arg == null) continue;

                var validator = _validatorFactory.GetValidator(arg.GetType());

                if (validator != null)
                {
                    var validationResult = await validator.ValidateAsync(new ValidationContext<object>(arg));
                    if (!validationResult.IsValid)
                    {
                        context.Result = new BadRequestObjectResult(validationResult.Errors);
                        return;
                    }
                }
            }

            await next();
        }
    }
}
