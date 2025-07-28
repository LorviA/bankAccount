using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using bankAccount.Exceptions;
using System.Net;
using System.Text.Json;

namespace bankAccount;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

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