using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting; // Add this using directive
using Microsoft.Extensions.Hosting; // Add this using directive
using Microsoft.Extensions.Logging; // Add this using directive

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
                // Always log the full exception details
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                context.Response.ContentType = "application/json";

                ApiException response;

                // Check for your custom validation exception
                if (ex is CustomValidationException validationException)
                {
                    context.Response.StatusCode = validationException.StatusCode;
                    response = new ApiException(
                        validationException.StatusCode,
                        validationException.Message,
                        _env.IsDevelopment() ? validationException.StackTrace : null // Only show stack trace in development
                    );
                }
                else
                {
                    // For all other exceptions (including DbUpdateException if not caught specifically
                    // in your repository, or other unhandled application errors)
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = _env.IsDevelopment()
                        ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                        : new ApiException(context.Response.StatusCode, "An unexpected error occurred. Please try again later.");
                }

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}