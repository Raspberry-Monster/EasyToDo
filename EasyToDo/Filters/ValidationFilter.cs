using EasyToDo.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EasyToDo.Filters
{
    public sealed class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values.Where(argument => argument != null))
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(argument!.GetType());
                if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
                {
                    continue;
                }

                var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument), context.HttpContext.RequestAborted);
                if (validationResult.IsValid)
                {
                    continue;
                }

                var errors = validationResult.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
                context.Result = new BadRequestObjectResult(new ApiResponse<Dictionary<string, string[]>>
                {
                    Success = false,
                    Message = "Request validation failed.",
                    Data = errors
                });
                return;
            }

            await next();
        }
    }
}
