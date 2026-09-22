using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Common.MediatR;
using Matchi.Application.Features.Requests;
using Matchi.Application.Features.Requests.Commands.CreateRequest;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Matchi.Domain.Locations;

namespace Matchi.Application.Tests;

public sealed class CreateRequestCommandValidatorTests
{
    private static CreateRequestCommandValidator Validator(
        FakeRequestRepository? requests = null,
        FakeLocationReadRepository? locations = null)
    {
        var repo = requests ?? CatalogRepo();
        return new CreateRequestCommandValidator(repo, locations ?? CatalogLocations());
    }

    private static FakeLocationReadRepository CatalogLocations()
    {
        var locations = new FakeLocationReadRepository();
        locations.Provinces.Add(new LocationProvince("تهران", "THR").WithId(1));
        locations.Provinces.Add(new LocationProvince("البرز", "ALB").WithId(2));
        locations.Cities.Add(new LocationCity(1, "تهران").WithId(10));
        locations.Cities.Add(new LocationCity(2, "کرج").WithId(11));
        locations.Districts.Add(new LocationDistrict(10, "سعادت‌آباد").WithId(120));
        locations.Districts.Add(new LocationDistrict(11, "گلشهر").WithId(121));
        return locations;
    }

    private static RequestLocationDto ValidLocation() =>
        new(1, 10, 120, "خیابان مثال", 35.7, 51.3);

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
            error => error.ErrorMessage == "The selected product was not found or is inactive.");
    }

    [Fact]
    public async Task Product_from_another_category_fails_validation()
    {
        var command = new CreateRequestCommand(
            "Product",
            "Need a part",
            Products: [new RequestProductLineDto(20, 99, 1m)]);

        var result = await Validator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "The selected product does not belong to the specified product category.");
    }

    [Fact]
    public async Task Inactive_product_fails_validation()
    {
        var requests = CatalogRepo();
        requests.ProductsById[20].SetActive(false);

        var command = new CreateRequestCommand(
            "Product",
            "Need a part",
            Products: [new RequestProductLineDto(20, null, 1m)]);

        var result = await Validator(requests).ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "The selected product was not found or is inactive.");
    }

    [Fact]
    public async Task Deleted_product_fails_validation()
    {
        var requests = CatalogRepo();
        requests.ProductsById[20].SoftDelete();

        var command = new CreateRequestCommand(
            "Product",
            "Need a part",
            Products: [new RequestProductLineDto(20, null, 1m)]);

        var result = await Validator(requests).ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "The selected product was not found or is inactive.");
    }

    [Fact]
    public async Task Valid_product_attribute_succeeds()
    {
        var requests = CatalogRepo();
        var attribute = new ProductAttribute(3, "برند", "ac_brand", "Select").WithId(201);
        requests.ProductAttributes[(3, 201)] = attribute;

        var command = new CreateRequestCommand(
            "Product",
            "Need a part",
            Products:
            [
                new RequestProductLineDto(
                    20,
                    3,
                    1m,
                    Attributes: [new RequestProductAttributeDto(201, "gree")])
            ]);

        var result = await Validator(requests).ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Attribute_from_another_category_fails_validation()
    {
        var requests = CatalogRepo();
        requests.ProductAttributes[(9, 201)] = new ProductAttribute(9, "برند", "other_brand", "Select").WithId(201);

        var command = new CreateRequestCommand(
            "Product",
            "Need a part",
            Products:
            [
                new RequestProductLineDto(
                    20,
                    3,
                    1m,
                    Attributes: [new RequestProductAttributeDto(201, "gree")])
            ]);

        var result = await Validator(requests).ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "Product attribute does not belong to the product category for this line.");
    }

    [Fact]
    public async Task Inactive_product_attribute_fails_validation()
    {
        var requests = CatalogRepo();
        var attribute = new ProductAttribute(3, "برند", "ac_brand", "Select").WithId(201);
        attribute.SetActive(false);
        requests.ProductAttributes[(3, 201)] = attribute;

        var command = new CreateRequestCommand(
            "Product",
            "Need a part",
            Products:
            [
                new RequestProductLineDto(
                    20,
                    3,
                    1m,
                    Attributes: [new RequestProductAttributeDto(201, "gree")])
            ]);

        var result = await Validator(requests).ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "Product attribute does not belong to the product category for this line.");
    }

    [Fact]
    public async Task Deleted_product_attribute_fails_validation()
    {
        var requests = CatalogRepo();
        var attribute = new ProductAttribute(3, "برند", "ac_brand", "Select").WithId(201);
        attribute.SoftDelete();
        requests.ProductAttributes[(3, 201)] = attribute;

        var command = new CreateRequestCommand(
            "Product",
            "Need a part",
            Products:
            [
                new RequestProductLineDto(
                    20,
                    3,
                    1m,
                    Attributes: [new RequestProductAttributeDto(201, "gree")])
            ]);

        var result = await Validator(requests).ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == "Product attribute does not belong to the product category for this line.");
    }

    [Fact]
    public async Task Create_request_with_service_product_and_attributes_is_accepted()
    {
        var requests = CatalogRepo();
        requests.ServiceAttributes[(10, 77)] = new ServiceAttribute(10, "برند", "ac_brand", "Select").WithId(77);
        requests.ProductAttributes[(3, 201)] = new ProductAttribute(3, "برند", "ac_brand", "Select").WithId(201);

        var users = new StubUserRepository(new User("09120000000").WithId(5));
        var handler = new CreateRequestCommandHandler(new FakeCurrentUser(5), users, requests);

        var id = await handler.Handle(
            new CreateRequestCommand(
                "Hybrid",
                "نصب و تامین قطعه",
                Services:
                [
                    new RequestServiceLineDto(
                        10,
                        1m,
                        Attributes: [new RequestServiceAttributeDto(77, "lg")])
                ],
                Products:
                [
                    new RequestProductLineDto(
                        20,
                        3,
                        1m,
                        Attributes: [new RequestProductAttributeDto(201, "gree")])
                ]),
            CancellationToken.None);

        Assert.True(id > 0);
        var created = Assert.Single(requests.Added);
        Assert.Single(created.Services);
        Assert.Single(created.Products);
        Assert.Single(created.Services.First().Attributes);
        Assert.Single(created.Products.First().Attributes);
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

    [Fact]
    public async Task Create_request_with_valid_location_succeeds()
    {
        var requests = CatalogRepo();
        var handler = new CreateRequestCommandHandler(
            new FakeCurrentUser(5),
            new StubUserRepository(new User("09120000000").WithId(5)),
            requests);

        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(10, 1m)],
            Location: ValidLocation());

        var result = await Validator(requests).ValidateAsync(command);
        Assert.True(result.IsValid);

        var id = await handler.Handle(command, CancellationToken.None);
        Assert.True(id > 0);

        var location = Assert.Single(requests.Added.Single().Locations);
        Assert.Equal(1, location.ProvinceId);
        Assert.Equal(10, location.CityId);
        Assert.Equal(120, location.DistrictId);
        Assert.Equal("خیابان مثال", location.Address);
    }

    [Fact]
    public async Task Invalid_province_fails_validation()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(10, 1m)],
            Location: new RequestLocationDto(99, 10, 120, null, 35.7, 51.3));

        var result = await Validator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "استان انتخاب شده معتبر نیست");
    }

    [Fact]
    public async Task City_from_another_province_fails_validation()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(10, 1m)],
            Location: new RequestLocationDto(1, 11, 121, null, 35.7, 51.3));

        var result = await Validator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "شهر انتخاب شده متعلق به این استان نیست");
    }

    [Fact]
    public async Task District_from_another_city_fails_validation()
    {
        var command = new CreateRequestCommand(
            "Service",
            "Need plumbing",
            Services: [new RequestServiceLineDto(10, 1m)],
            Location: new RequestLocationDto(1, 10, 121, null, 35.7, 51.3));

        var result = await Validator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "محله انتخاب شده متعلق به این شهر نیست");
    }
}

internal sealed class StubUserRepository : IUserRepository
{
    public StubUserRepository(User user)
    {
        User = user;
    }

    public User User { get; }

    public Task<User?> GetByMobileAsync(string mobile, CancellationToken cancellationToken = default) =>
        Task.FromResult<User?>(User.Mobile == mobile ? User : null);

    public Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default) =>
        Task.FromResult<User?>(User.Id == userId ? User : null);

    public Task<User> AddAsync(User user, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}
