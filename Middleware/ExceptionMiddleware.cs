
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.SecurityTokenService;
using OpenQA.Selenium;
using Raven.Client.Exceptions;
using VehicleServiceBook.Middleware; // Ensure this matches your actual namespace for exceptions

namespace VehicleServiceBook.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                context.Response.ContentType = "application/json";

                var statusCode = (int)HttpStatusCode.InternalServerError;
                var message = "An unexpected error occurred. Please try again later.";
                string? details = null;

                switch (ex)
                {
                    case CustomValidationException validationException:
                        statusCode = validationException.StatusCode;
                        message = validationException.Message;
                        details = _env.IsDevelopment() ? validationException.StackTrace : null;
                        break;

                    case Microsoft.IdentityModel.SecurityTokenService.BadRequestException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        message = ex.Message;
                        details = _env.IsDevelopment() ? ex.StackTrace : null;
                        break;

                    case NotFoundException:
                        statusCode = (int)HttpStatusCode.NotFound;
                        message = ex.Message;
                        details = _env.IsDevelopment() ? ex.StackTrace : null;
                        break;

                    case ConflictException:
                        statusCode = (int)HttpStatusCode.Conflict;
                        message = ex.Message;
                        details = _env.IsDevelopment() ? ex.StackTrace : null;
                        break;

                    default:
                        if (_env.IsDevelopment())
                        {
                            message = ex.Message;
                            details = ex.StackTrace;
                        }
                        break;
                }

                var response = new ApiException(statusCode, message, details);
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(json);
            }
        }
    }
}
