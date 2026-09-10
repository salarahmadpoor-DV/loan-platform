using FluentValidation;

namespace Matchi.Application.Common;

public static class ExceptionHttpMapper
{
    public static ExceptionHttpResult Map(
        Exception? exception,
        string traceId,
        bool exposeInternalDetails)
    {
        var status = exception switch
        {
            ValidationException => 400,
            UnauthorizedAccessException => 401,
            KeyNotFoundException => 404,
            ConflictException => 409,
            _ => 500
        };

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return new ExceptionHttpResult(
                status,
                "Validation failed",
                "One or more validation errors occurred.",
                traceId,
                errors);
        }

        if (status == 401)
        {
            return new ExceptionHttpResult(
                status,
                "Unauthorized",
                string.IsNullOrWhiteSpace(exception?.Message) ? "Unauthorized." : exception.Message,
                traceId,
                null);
        }

        if (status == 404)
        {
            return new ExceptionHttpResult(
                status,
                "Not Found",
                string.IsNullOrWhiteSpace(exception?.Message) ? "The requested resource was not found." : exception.Message,
                traceId,
                null);
        }

        if (status == 409)
        {
            return new ExceptionHttpResult(
                status,
                "Conflict",
                string.IsNullOrWhiteSpace(exception?.Message) ? "The request conflicts with the current state." : exception.Message,
                traceId,
                null);
        }

        if (exposeInternalDetails && exception is not null)
        {
            return new ExceptionHttpResult(
                status,
                exception.GetType().Name,
                exception.Message,
                traceId,
                null);
        }

        return new ExceptionHttpResult(
            status,
            "Internal Server Error",
            "An unexpected error occurred.",
            traceId,
            null);
    }
}

public sealed record ExceptionHttpResult(
    int Status,
    string Title,
    string Detail,
    string TraceId,
    IReadOnlyDictionary<string, string[]>? Errors);
