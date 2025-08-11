using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using bankAccount.Exceptions;
using System.Text.Json;

namespace bankAccount;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            ValidationAppException => (StatusCodes.Status422UnprocessableEntity, "Validation Error"),
            _ => (StatusCodes.Status500InternalServerError, "Server Error")
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        if (exception is ValidationAppException validationException)
        {
            var problemDetails = new ProblemDetails
            {
                Title = title,
                Status = statusCode,
                Detail = "One or more validation errors occurred",
                Extensions = { ["errors"] = validationException.Errors }
            };

            await JsonSerializer.SerializeAsync(
                httpContext.Response.Body,
                problemDetails,
                cancellationToken: cancellationToken);

            return true;
        }

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Title = title,
                Status = statusCode,
                Detail = exception.Message
            },
            Exception = exception
        });
    }
}