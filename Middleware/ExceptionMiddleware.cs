using System.Net;

using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using OpenQA.Selenium;

using Raven.Client.Exceptions;

namespace VehicleServiceBook.Middleware

{

    public class ExceptionMiddleware

    {

        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)

        {

            _next = next;

            _logger = logger;

        }

        public async Task InvokeAsync(HttpContext context)

        {

            try

            {

                await _next(context); // try to continue pipeline

            }

            catch (DbUpdateException dbEx)

            {

                _logger.LogError(dbEx, "Database update failed");

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                context.Response.ContentType = "application/json";

                var response = new

                {

                    statusCode = context.Response.StatusCode,

                    message = "A database error occurred.",

                    details = dbEx.InnerException?.Message ?? dbEx.Message

                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Unhandled error occurred");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                context.Response.ContentType = "application/json";

                var response = new

                {

                    statusCode = context.Response.StatusCode,

                    message = "An unexpected error occurred.",

                    details = ex.Message

                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));

            }

        }

    }

}

