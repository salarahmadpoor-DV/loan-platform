using FluentValidation;
using Matchi.Application.Common.MediatR;
using Matchi.Application.Features.Requests;
using Matchi.Application.Features.Requests.Commands.CreateRequest;
using Matchi.Domain.Entities;

namespace Matchi.Application.Tests;

public sealed class CreateRequestCommandValidatorTests
{
    private static CreateRequestCommandValidator Validator(FakeRequestRepository? requests = null)
    {
        var repo = requests ?? CatalogRepo();
        return new CreateRequestCommandValidator(repo);
    }

    private static FakeRequestRepository CatalogRepo()
    {
        var requests = new FakeRequestRepository();
        requests.ExistingServiceIds.Add(10);
        requests.ProductsById[20] = new Product(3, "Item").WithId(20);
        requests.ExistingCategoryIds.Add(3);
        return requests;
    }

    [Fact]
    public async Task Valid_Service_request_passes()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(10, 1m)]);

        var result = await Validator().ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Valid_Product_request_passes()
    {
        var command = new CreateRequestCommand(
            "Product",
            "Need a part",
            Products: [new RequestProductLineDto(20, null, 2m)]);

        var result = await Validator().ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Valid_Hybrid_request_passes()
    {
        var command = new CreateRequestCommand(
            "Hybrid",
            "Install and supply",
            Services: [new RequestServiceLineDto(10, 1m)],
            Products: [new RequestProductLineDto(20, 3, 1m)]);

        var result = await Validator().ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Invalid_Service_item_fails_validation()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(0, 1m)]);

        var result = await Validator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName.Contains("Services", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Invalid_Product_item_fails_validation()
    {
        var command = new CreateRequestCommand(
            "Product",
            "Need a part",
            Products: [new RequestProductLineDto(null, null, 1m)]);

        var result = await Validator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName.Contains("Products", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Invalid_Service_catalog_data_fails_validation()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(999, 1m)]);

        var result = await Validator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "The selected service was not found or is inactive.");
    }

    [Fact]
    public async Task Invalid_Product_catalog_data_fails_validation()
    {
        var command = new CreateRequestCommand(
            "Product",
            "Need a part",
            Products: [new RequestProductLineDto(999, null, 1m)]);

        var result = await Validator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "Product, product category, or product attribute is invalid for this line.");
    }

    [Fact]
    public async Task Invalid_collection_produces_validation_failure_not_exception()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(0, 0m)]);

        var exception = await Record.ExceptionAsync(() => Validator().ValidateAsync(command));
        Assert.Null(exception);

        var result = await Validator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Async_catalog_validation_via_ValidateAsync_does_not_throw_sync_invocation_exception()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(10, 1m)]);

        var exception = await Record.ExceptionAsync(() => Validator().ValidateAsync(command));

        Assert.Null(exception);
    }

    [Fact]
    public void Synchronous_Validate_throws_because_catalog_rules_are_async()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(10, 1m)]);

        Assert.Throws<AsyncValidatorInvokedSynchronouslyException>(
            () => Validator().Validate(command));
    }

    [Fact]
    public async Task ValidationBehavior_runs_async_rules_and_returns_validation_failures()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(999, 1m)]);
        var behavior = new ValidationBehavior<CreateRequestCommand, long>([Validator()]);

        var exception = await Record.ExceptionAsync(() =>
            behavior.Handle(command, _ => Task.FromResult(1L), CancellationToken.None));

        Assert.IsNotType<AsyncValidatorInvokedSynchronouslyException>(exception);
        var validationException = Assert.IsType<ValidationException>(exception);
        Assert.Contains(
            validationException.Errors,
            error => error.ErrorMessage == "The selected service was not found or is inactive.");
    }

    [Fact]
    public async Task ValidationBehavior_allows_valid_request_without_sync_invocation_exception()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(10, 1m)]);
        var behavior = new ValidationBehavior<CreateRequestCommand, long>([Validator()]);

        var exception = await Record.ExceptionAsync(() =>
            behavior.Handle(command, _ => Task.FromResult(42L), CancellationToken.None));

        Assert.Null(exception);
        var id = await behavior.Handle(command, _ => Task.FromResult(42L), CancellationToken.None);
        Assert.Equal(42L, id);
    }
}
