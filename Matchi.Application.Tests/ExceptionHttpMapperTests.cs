using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common;

namespace Matchi.Application.Tests;

public sealed class ExceptionHttpMapperTests
{
    [Fact]
    public void Production_unexpected_errors_hide_internal_details()
    {
        var result = ExceptionHttpMapper.Map(
            new InvalidOperationException("secret schema detail"),
            "trace-1",
            exposeInternalDetails: false);

        Assert.Equal(500, result.Status);
        Assert.Equal("Internal Server Error", result.Title);
        Assert.Equal("An unexpected error occurred.", result.Detail);
        Assert.Equal("trace-1", result.TraceId);
        Assert.DoesNotContain("secret schema detail", result.Detail, StringComparison.Ordinal);
        Assert.DoesNotContain("InvalidOperationException", result.Title, StringComparison.Ordinal);
    }

    [Fact]
    public void Development_unexpected_errors_may_include_details()
    {
        var result = ExceptionHttpMapper.Map(
            new InvalidOperationException("dev detail"),
            "trace-2",
            exposeInternalDetails: true);

        Assert.Equal(500, result.Status);
        Assert.Equal("InvalidOperationException", result.Title);
        Assert.Equal("dev detail", result.Detail);
        Assert.Equal("trace-2", result.TraceId);
    }

    [Fact]
    public void Validation_errors_remain_400()
    {
        var exception = new ValidationException(new[]
        {
            new ValidationFailure("dealId", "required")
        });

        var result = ExceptionHttpMapper.Map(exception, "trace-3", exposeInternalDetails: false);

        Assert.Equal(400, result.Status);
        Assert.Equal("One or more validation errors occurred.", result.Detail);
        Assert.NotNull(result.Errors);
        Assert.Equal("required", result.Errors!["dealId"][0]);
    }

    [Fact]
    public void Not_found_remains_404()
    {
        var result = ExceptionHttpMapper.Map(
            new KeyNotFoundException("Deal was not found."),
            "trace-4",
            exposeInternalDetails: false);

        Assert.Equal(404, result.Status);
        Assert.Equal("Deal was not found.", result.Detail);
    }

    [Fact]
    public void Unique_and_concurrency_conflicts_are_409()
    {
        var result = ExceptionHttpMapper.Map(
            new ConflictException("A deal already exists for this proposal."),
            "trace-5",
            exposeInternalDetails: false);

        Assert.Equal(409, result.Status);
        Assert.Equal("Conflict", result.Title);
        Assert.Equal("A deal already exists for this proposal.", result.Detail);
        Assert.DoesNotContain("SqlException", result.Detail, StringComparison.Ordinal);
    }

    [Fact]
    public void Production_500_remains_sanitized_when_conflict_is_not_used()
    {
        var result = ExceptionHttpMapper.Map(
            new Exception("inner connection string"),
            "trace-6",
            exposeInternalDetails: false);

        Assert.Equal(500, result.Status);
        Assert.Equal("Internal Server Error", result.Title);
        Assert.Equal("An unexpected error occurred.", result.Detail);
        Assert.DoesNotContain("connection string", result.Detail, StringComparison.Ordinal);
    }
}
