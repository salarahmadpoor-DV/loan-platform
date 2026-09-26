using FluentValidation;
using Matchi.Application.Features.Requests.Commands.CreateRequest;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Features.Requests;

internal static class RequestWriteRules
{
    public static readonly string[] AllowedTypes = ["Service", "Product", "Hybrid"];

    public static void Apply(
        AbstractValidator<CreateRequestCommand> validator,
        IRequestRepository requests,
        ILocationReadRepository locations)
    {
        validator.RuleFor(x => x.RequestType)
            .NotEmpty().WithMessage("Request type is required.")
            .Must(type => AllowedTypes.Contains(type, StringComparer.Ordinal))
            .WithMessage("Request type must be Service, Product, or Hybrid.");

        validator.RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(500).WithMessage("Title must be at most 500 characters.");

        validator.RuleFor(x => x)
            .Must(x =>
            {
                var type = x.RequestType;
                var serviceCount = x.Services?.Count ?? 0;
                var productCount = x.Products?.Count ?? 0;

                return type switch
                {
                    "Service" => serviceCount >= 1 && productCount == 0,
                    "Product" => productCount >= 1 && serviceCount == 0,
                    "Hybrid" => serviceCount >= 1 && productCount >= 1,
                    _ => true
                };
            })
            .WithMessage("Service requests require services only, product requests require products only, and hybrid requests require both.");

        // RuleForEach must bind a member-access expression. Captured Func invoke + ?? Empty()
        // cannot infer a property name (FluentValidation 12 throws InvalidOperationException → HTTP 500).
        validator.When(x => x.Services is not null, () =>
        {
            validator.RuleForEach(x => x.Services)
                .SetValidator(new RequestServiceLineValidator(requests));
        });

        validator.When(x => x.Products is not null, () =>
        {
            validator.RuleForEach(x => x.Products)
                .SetValidator(new RequestProductLineValidator(requests));
        });

        validator.When(x => x.Location is not null, () =>
        {
            validator.RuleFor(x => x.Location!.Address).MaximumLength(1000);
            validator.RuleFor(x => x.Location!)
                .SetValidator(new RequestLocationDtoValidator(locations));
        });

        validator.When(x => x.Schedule is not null, () =>
        {
            validator.RuleFor(x => x.Schedule!)
                .Must(s => s.TimeFrom is null || s.TimeTo is null || s.TimeFrom < s.TimeTo)
                .WithMessage("Schedule timeFrom must be earlier than timeTo.");
        });
    }
}

internal sealed class RequestLocationDtoValidator : AbstractValidator<RequestLocationDto>
{
    public RequestLocationDtoValidator(ILocationReadRepository locations)
    {
        RuleFor(x => x.ProvinceId)
            .GreaterThan(0).WithMessage("استان انتخاب شده معتبر نیست");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("شهر انتخاب شده متعلق به این استان نیست");

        RuleFor(x => x.DistrictId)
            .GreaterThan(0).WithMessage("محله انتخاب شده متعلق به این شهر نیست");

        RuleFor(x => x)
            .MustAsync(async (location, cancellationToken) =>
            {
                if (location.ProvinceId <= 0)
                    return true;

                var province = await locations.FindProvinceByIdAsync(location.ProvinceId, cancellationToken);
                return province is { IsActive: true };
            })
            .WithMessage("استان انتخاب شده معتبر نیست");

        RuleFor(x => x)
            .MustAsync(async (location, cancellationToken) =>
            {
                if (location.CityId <= 0)
                    return true;

                var city = await locations.FindCityByIdAsync(location.CityId, cancellationToken);
                return city is { IsActive: true } && city.ProvinceId == location.ProvinceId;
            })
            .WithMessage("شهر انتخاب شده متعلق به این استان نیست");

        RuleFor(x => x)
            .MustAsync(async (location, cancellationToken) =>
            {
                if (location.DistrictId <= 0)
                    return true;

                var district = await locations.FindDistrictByIdAsync(location.DistrictId, cancellationToken);
                return district is { IsActive: true } && district.CityId == location.CityId;
            })
            .WithMessage("محله انتخاب شده متعلق به این شهر نیست");
    }
}

internal sealed class RequestServiceLineValidator : AbstractValidator<RequestServiceLineDto>
{
    public RequestServiceLineValidator(IRequestRepository requests)
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0).WithMessage("Service id is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x)
            .MustAsync(async (line, cancellationToken) =>
                await requests.ServiceExistsAsync(line.ServiceId, cancellationToken))
            .WithMessage("The selected service was not found or is inactive.");

        RuleFor(x => x.Attributes)
            .Must(attributes =>
            {
                var ids = (attributes ?? Array.Empty<RequestServiceAttributeDto>())
                    .Select(a => a.ServiceAttributeId)
                    .ToList();
                return ids.Count == ids.Distinct().Count();
            })
            .WithMessage("Service attributes must be unique per service line.");

        When(x => x.Attributes is not null, () =>
        {
            RuleForEach(x => x.Attributes)
                .ChildRules(attribute =>
                {
                    attribute.RuleFor(a => a.ServiceAttributeId)
                        .GreaterThan(0).WithMessage("Service attribute id is required.");

                    attribute.RuleFor(a => a.Value)
                        .MaximumLength(2000);
                });
        });

        RuleFor(x => x)
            .MustAsync(async (line, cancellationToken) =>
            {
                foreach (var attribute in line.Attributes ?? Array.Empty<RequestServiceAttributeDto>())
                {
                    var definition = await requests.GetServiceAttributeAsync(
                        line.ServiceId,
                        attribute.ServiceAttributeId,
                        cancellationToken);

                    if (definition is null)
                        return false;
                }

                return true;
            })
            .WithMessage("Service attribute does not belong to the selected service.");
    }
}

internal sealed class RequestProductLineValidator : AbstractValidator<RequestProductLineDto>
{
    public RequestProductLineValidator(IRequestRepository requests)
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Unit)
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x)
            .Must(line => line.ProductId is > 0 || line.ProductCategoryId is > 0)
            .WithMessage("A product line must reference a product or a product category.");

        RuleFor(x => x.Attributes)
            .Must(attributes =>
            {
                var ids = (attributes ?? Array.Empty<RequestProductAttributeDto>())
                    .Select(a => a.ProductAttributeId)
                    .ToList();
                return ids.Count == ids.Distinct().Count();
            })
            .WithMessage("Product attributes must be unique per product line.");

        When(x => x.Attributes is not null, () =>
        {
            RuleForEach(x => x.Attributes)
                .ChildRules(attribute =>
                {
                    attribute.RuleFor(a => a.ProductAttributeId)
                        .GreaterThan(0).WithMessage("Product attribute id is required.");

                    attribute.RuleFor(a => a.Value)
                        .MaximumLength(2000);
                });
        });

        RuleFor(x => x.ProductId)
            .MustAsync(async (line, productId, cancellationToken) =>
            {
                if (productId is not > 0)
                    return true;

                return await requests.GetProductAsync(productId.Value, cancellationToken) is not null;
            })
            .WithMessage("The selected product was not found or is inactive.");

        RuleFor(x => x)
            .MustAsync(async (line, cancellationToken) =>
            {
                if (line.ProductId is not > 0 || line.ProductCategoryId is not > 0)
                    return true;

                var product = await requests.GetProductAsync(line.ProductId.Value, cancellationToken);
                if (product is null)
                    return true;

                return product.CategoryId == line.ProductCategoryId.Value;
            })
            .WithMessage("The selected product does not belong to the specified product category.");

        RuleFor(x => x.ProductCategoryId)
            .MustAsync(async (line, categoryId, cancellationToken) =>
            {
                if (categoryId is not > 0)
                    return true;

                return await requests.ProductCategoryExistsAsync(categoryId.Value, cancellationToken);
            })
            .WithMessage("The selected product category was not found or is inactive.");

        RuleFor(x => x)
            .MustAsync(async (line, cancellationToken) =>
            {
                long? categoryId = line.ProductCategoryId is > 0 ? line.ProductCategoryId : null;

                if (line.ProductId is > 0)
                {
                    var product = await requests.GetProductAsync(line.ProductId.Value, cancellationToken);
                    if (product is null)
                        return true;

                    categoryId = product.CategoryId;
                }

                foreach (var attribute in line.Attributes ?? Array.Empty<RequestProductAttributeDto>())
                {
                    if (categoryId is null)
                        return false;

                    var definition = await requests.GetProductAttributeAsync(
                        categoryId.Value,
                        attribute.ProductAttributeId,
                        cancellationToken);

                    if (definition is null)
                        return false;
                }

                return true;
            })
            .WithMessage("Product attribute does not belong to the product category for this line.");
    }
}
