// File: Validation/CustomClientValidationDataFilter.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Catalog_Service.src.CrossCutting.Validation
{
    public class CustomClientValidationDataFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var clientValidationRules = new List<object>();
                var modelType = objectResult.Value.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(modelType);
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var descriptor = validator.CreateDescriptor();
                    var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

                    foreach (var property in properties)
                    {
                        // --- کد اصلاح شده و نهایی ---
                        var rulesForMember = descriptor.GetRulesForMember(property.Name);

                        foreach (var rule in rulesForMember)
                        {
                            // هر rule شامل چندین component است. ما از component ها اطلاعات validator را استخراج می‌کنیم.
                            foreach (var component in rule.Components)
                            {
                                // نام نوع validator را از کامپوننت استخراج می‌کنیم.
                                var validatorTypeName = component.Validator.GetType().Name;

                                // یک پیام خطای عمومی بر اساس نوع validator ایجاد می‌کنیم.
                                var errorMessage = validatorTypeName switch
                                {
                                    "NotNullValidator" => $"{property.Name} should not be empty.",
                                    "NotEmptyValidator" => $"{property.Name} should not be empty.",
                                    "EmailValidator" => $"{property.Name} is not a valid email address.",
                                    "LengthValidator" => $"{property.Name} has an invalid length.",
                                    "RegularExpressionValidator" => $"{property.Name} has an invalid format.",
                                    _ => $"{property.Name} is not valid."
                                };

                                clientValidationRules.Add(new
                                {
                                    propertyName = property.Name,
                                    validatorType = validatorTypeName,
                                    errorMessage = errorMessage
                                });
                            }
                        }
                        // --- پایان کد اصلاح شده ---
                    }
                }

                // Add metadata to the response header for client-side scripts to pick up
                if (clientValidationRules.Any())
                {
                    context.HttpContext.Response.Headers.Add("X-Validation-Rules", JsonSerializer.Serialize(clientValidationRules));
                }
            }
        }

        public void OnResultExecuted(ResultExecutedContext context) { }
    }
}