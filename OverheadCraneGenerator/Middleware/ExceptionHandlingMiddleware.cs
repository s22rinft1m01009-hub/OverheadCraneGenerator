using CraneGenerator.Application.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace OverheadCraneGenerator.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DesignValidationException exception)
            {
                await WriteProblemDetailsAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    "Design validation failed.",
                    exception.Errors);
            }
            catch (ArgumentException exception)
            {
                await WriteProblemDetailsAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    exception.Message,
                    [exception.Message]);
            }
            catch (DbUpdateConcurrencyException exception)
            {
                _logger.LogInformation(exception, "A concurrent database update prevented the request.");
                await WriteProblemDetailsAsync(
                    context,
                    StatusCodes.Status409Conflict,
                    "The resource was changed by another request.");
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception occurred.");

                await WriteProblemDetailsAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        private static async Task WriteProblemDetailsAsync(
            HttpContext context,
            int statusCode,
            string title,
            IEnumerable<string>? errors = null)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "The request could not be completed."
                    : title,
                Instance = context.Request.Path
            };

            if (errors is not null)
                problem.Extensions["errors"] = errors;

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
