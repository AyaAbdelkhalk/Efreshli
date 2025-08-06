using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Efreshli.Common
{
    public class ValidateModelAsyncFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidateModelAsyncFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(argument);
                    var result = await validator.ValidateAsync(validationContext);

                    if (!result.IsValid)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                }
            }
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }
            await next();
        }
    }
}