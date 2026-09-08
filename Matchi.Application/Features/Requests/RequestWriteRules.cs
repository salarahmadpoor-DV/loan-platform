using FluentValidation;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Features.Requests;

internal static class RequestWriteRules
{
    public static readonly string[] AllowedTypes = ["Service", "Product", "Hybrid"];

    public static void Apply<T>(
        AbstractValidator<T> validator,
        Func<T, string> requestType,
        Func<T, string> title,
        Func<T, IReadOnlyList<RequestServiceLineDto>?> services,
        Func<T, IReadOnlyList<RequestProductLineDto>?> products,
        Func<T, RequestLocationDto?> location,
        Func<T, RequestScheduleDto?> schedule,
        IRequestRepository requests)
    {
        validator.RuleFor(x => requestType(x))
            .NotEmpty().WithMessage("Request type is required.")
            .Must(type => AllowedTypes.Contains(type, StringComparer.Ordinal))
            .WithMessage("Request type must be Service, Product, or Hybrid.");

        validator.RuleFor(x => title(x))
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(500).WithMessage("Title must be at most 500 characters.");

        validator.RuleFor(x => x)
            .Must(x =>
            {
                var type = requestType(x);
                var serviceCount = services(x)?.Count ?? 0;
                var productCount = products(x)?.Count ?? 0;

                return type switch
                {
                    "Service" => serviceCount >= 1 && productCount == 0,
                    "Product" => productCount >= 1 && serviceCount == 0,
                    "Hybrid" => serviceCount >= 1 && productCount >= 1,
                    _ => true
                };
            })
            .WithMessage("Service requests require services only, product requests require products only, and hybrid requests require both.");

        validator.RuleForEach(x => services(x) ?? Array.Empty<RequestServiceLineDto>())
            .SetValidator(new RequestServiceLineValidator(requests));

        validator.RuleForEach(x => products(x) ?? Array.Empty<RequestProductLineDto>())
            .SetValidator(new RequestProductLineValidator(requests));

        validator.When(x => location(x) is not null, () =>
        {
            validator.RuleFor(x => location(x)!.Province).MaximumLength(100);
            validator.RuleFor(x => location(x)!.City).MaximumLength(100);
            validator.RuleFor(x => location(x)!.District).MaximumLength(100);
            validator.RuleFor(x => location(x)!.Address).MaximumLength(1000);
        });

        validator.When(x => schedule(x) is not null, () =>
        {
            validator.RuleFor(x => schedule(x)!)
                .Must(s => s.TimeFrom is null || s.TimeTo is null || s.TimeFrom < s.TimeTo)
                .WithMessage("Schedule timeFrom must be earlier than timeTo.");
        });
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

        RuleForEach(x => x.Attributes ?? Array.Empty<RequestServiceAttributeDto>())
            .ChildRules(attribute =>
            {
                attribute.RuleFor(a => a.ServiceAttributeId)
                    .GreaterThan(0).WithMessage("Service attribute id is required.");

                attribute.RuleFor(a => a.Value)
                    .MaximumLength(2000);
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

        RuleForEach(x => x.Attributes ?? Array.Empty<RequestProductAttributeDto>())
            .ChildRules(attribute =>
            {
                attribute.RuleFor(a => a.ProductAttributeId)
                    .GreaterThan(0).WithMessage("Product attribute id is required.");

                attribute.RuleFor(a => a.Value)
                    .MaximumLength(2000);
            });

        RuleFor(x => x)
            .MustAsync(async (line, cancellationToken) =>
            {
                long? categoryId = line.ProductCategoryId is > 0 ? line.ProductCategoryId : null;

                if (line.ProductId is > 0)
                {
                    var product = await requests.GetProductAsync(line.ProductId.Value, cancellationToken);
                    if (product is null)
                        return false;

                    if (categoryId is not null && categoryId.Value != product.CategoryId)
                        return false;

                    categoryId = product.CategoryId;
                }
                else if (categoryId is not null)
                {
                    if (!await requests.ProductCategoryExistsAsync(categoryId.Value, cancellationToken))
                        return false;
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
            .WithMessage("Product, product category, or product attribute is invalid for this line.");
    }
}
