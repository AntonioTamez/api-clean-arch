using ApiCleanArch.Application.Users;
using ApiCleanArch.Domain.Shared;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ApiCleanArch.Api.Errors;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            UserNotFoundException e => (StatusCodes.Status404NotFound, "User not found", e.Message),
            EmailAlreadyInUseException e => (StatusCodes.Status409Conflict, "Email already in use", e.Message),
            DomainException e => (StatusCodes.Status400BadRequest, "Invalid request", e.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred", null as string),
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails { Status = status, Title = title, Detail = detail },
        });
    }
}
