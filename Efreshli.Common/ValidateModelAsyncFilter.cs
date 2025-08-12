using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Efreshli.Common
{
    //public class ValidateModelAsyncFilter : IAsyncActionFilter
    //{
    //    private readonly IServiceProvider _serviceProvider;

    //    public ValidateModelAsyncFilter(IServiceProvider serviceProvider)
    //    {
    //        _serviceProvider = serviceProvider;
    //    }
    //    public async Task OnActionExecutionAsync(ActionExecutingContext context,
    //        ActionExecutionDelegate next)
    //    {
    //        foreach (var argument in context.ActionArguments.Values)
    //        {
    //            if (argument == null) continue;

    //            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
    //            var validator = _serviceProvider.GetService(validatorType) as IValidator;

    //            if (validator != null)
    //            {
    //                var validationContext = new ValidationContext<object>(argument);
    //                var result = await validator.ValidateAsync(validationContext);

    //                if (!result.IsValid)
    //                {
    //                    foreach (var error in result.Errors)
    //                    {
    //                        context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    //                    }
    //                }
    //            }
    //        }
    //        if (!context.ModelState.IsValid)
    //        {
    //            context.Result = new BadRequestObjectResult(context.ModelState);
    //            return;
    //        }
    //        await next();
    //    }
    //}



    public class ValidateModelAsyncFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidateModelAsyncFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Skip validation if ModelState is already invalid (e.g., from [ApiController])
            if (!context.ModelState.IsValid)
            {
                await next();
                return;
            }

            // Validate all action arguments
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

            // If ModelState is invalid, handle response based on request type
            if (!context.ModelState.IsValid)
            {
                // Case 1: API request (Accept: application/json)
                if (IsApiRequest(context))
                {
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
                // Case 2: MVC request (form submission)
                else
                {
                    var controller = context.Controller as Controller;
                    if (controller != null)
                    {
                        // Get the first action argument (usually the DTO)
                        var model = context.ActionArguments.Values.FirstOrDefault();
                        context.Result = controller.View(model); // Re-render the view with errors
                    }
                    else
                    {
                        context.Result = new BadRequestResult(); // Fallback
                    }
                }
                return;
            }

            await next();
        }

        private bool IsApiRequest(ActionExecutingContext context)
        {
            // Check if the request explicitly asks for JSON
            if (context.HttpContext.Request.Headers.ContainsKey("Accept") &&
                context.HttpContext.Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                return true;
            }

            // Additional check for API-like requests (e.g., no Accept header but likely an API call)
            if (context.HttpContext.Request.Path.StartsWithSegments("/api"))
            {
                return true;
            }

            return false;
        }
    }


}