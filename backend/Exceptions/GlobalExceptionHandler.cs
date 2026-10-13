
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Operations_Platform_Assignment.Contracts;

namespace Operations_Platform_Assignment.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problem;

        switch (exception)
        {
            case OperationValidationException validationException:
                problem = new ValidationProblemDetails(
                    validationException.Errors.ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid operation parameters"
                };
                break;

            case HttpRequestException:
            case TaskCanceledException when
                !httpContext.RequestAborted.IsCancellationRequested:
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status502BadGateway,
                    Title = "External service error",
                    Detail = "An external service is unavailable."
                };
                break;

            default:
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal server error",
                    Detail = "An unexpected error occurred."
                };
                break;
        }

        if (problem.Status >= 500)
        {
            _logger.LogError(
                exception,
                "Request failed: {Path}",
                httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                "Request validation failed: {Message}",
                exception.Message);
        }

        httpContext.Response.StatusCode =
            problem.Status ?? StatusCodes.Status500InternalServerError;

        httpContext.Response.ContentType =
            "application/problem+json";

        // Serialize using the runtime type so ValidationProblemDetails
        // includes its Errors dictionary.
        if (problem is ValidationProblemDetails validationProblem)
        {
            await httpContext.Response.WriteAsJsonAsync(
                validationProblem,
                cancellationToken);
        }
        else
        {
            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken);
        }

        return true;
    }
}
