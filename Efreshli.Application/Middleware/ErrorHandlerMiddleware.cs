using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Efreshli.Application.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        // Change _localizer to be resolved per request instead of being a field
        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var localizer = context.RequestServices.GetRequiredService<IStringLocalizer<SharedResources>>();

            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var responseModel = new Response<string> { Succeeded = false };

                switch (error)
                {
                    case UnauthorizedAccessException:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        responseModel.StatusCode = HttpStatusCode.Unauthorized;
                        responseModel.Message = localizer[SharedResourcesKeys.Error.UnauthorizedAccess];
                        break;

                    case ValidationException validationEx:
                        response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                        responseModel.StatusCode = HttpStatusCode.UnprocessableEntity;
                        responseModel.Message = string.Join(" | ", validationEx.Errors.Select(err => $"{err.PropertyName}: {err.ErrorMessage}"));
                        break;

                    case KeyNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel.StatusCode = HttpStatusCode.NotFound;
                        responseModel.Message = localizer[SharedResourcesKeys.Error.DataNotFound];
                        break;

                    case DbUpdateException dbEx:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.StatusCode = HttpStatusCode.BadRequest;
                        responseModel.Message = localizer[SharedResourcesKeys.Error.GeneralError];
                        Console.WriteLine(responseModel.Message);
                        break;

                    case Exception e when e.GetType().Name == "ApiException":
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.StatusCode = HttpStatusCode.BadRequest;
                        responseModel.Message = localizer[SharedResourcesKeys.Error.GeneralError];
                        break;

                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.StatusCode = HttpStatusCode.InternalServerError;
                        responseModel.Message = localizer[SharedResourcesKeys.Error.GeneralError];
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }

        private string GetFullErrorMessage(Exception ex)
        {
            var sb = new StringBuilder();
            sb.Append(ex.Message);
            if (ex.InnerException != null)
                sb.AppendLine("\n").Append(ex.InnerException.Message);
            return sb.ToString();
        }
    }
}
