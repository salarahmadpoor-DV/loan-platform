using FluentValidation;
using FluentValidation.Results;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Features.Requests;

internal static class RequestAggregateApplier
{
    public static async Task ApplyAsync(
        Request request,
        IReadOnlyList<RequestServiceLineDto>? services,
        IReadOnlyList<RequestProductLineDto>? products,
        RequestLocationDto? location,
        RequestScheduleDto? schedule,
        IRequestRepository requests,
        CancellationToken cancellationToken)
    {
        var serviceLines = services ?? Array.Empty<RequestServiceLineDto>();
        var productLines = products ?? Array.Empty<RequestProductLineDto>();

        for (var i = 0; i < serviceLines.Count; i++)
        {
            var line = serviceLines[i];
            if (!await requests.ServiceExistsAsync(line.ServiceId, cancellationToken))
            {
                throw CreateValidationException(
                    $"Services[{i}].ServiceId",
                    "The selected service was not found or is inactive.");
            }

            var requestService = new RequestService(
                request.Id,
                line.ServiceId,
                line.Quantity,
                line.Description,
                line.DisplayOrder ?? i);

            var attributes = line.Attributes ?? Array.Empty<RequestServiceAttributeDto>();
            for (var a = 0; a < attributes.Count; a++)
            {
                var attribute = attributes[a];
                var definition = await requests.GetServiceAttributeAsync(
                    line.ServiceId,
                    attribute.ServiceAttributeId,
                    cancellationToken);

                if (definition is null)
                {
                    throw CreateValidationException(
                        $"Services[{i}].Attributes[{a}].ServiceAttributeId",
                        "Service attribute does not belong to the selected service.");
                }

                requestService.AddAttribute(attribute.ServiceAttributeId, attribute.Value);
            }

            request.AddService(requestService);
        }

        for (var i = 0; i < productLines.Count; i++)
        {
            var line = productLines[i];
            var categoryId = await ResolveProductCategoryIdAsync(line, requests, i, cancellationToken);

            var requestProduct = new RequestProduct(
                request.Id,
                line.ProductId,
                categoryId,
                line.Quantity,
                line.Unit,
                line.Description,
                line.DisplayOrder ?? i);

            var attributes = line.Attributes ?? Array.Empty<RequestProductAttributeDto>();
            for (var a = 0; a < attributes.Count; a++)
            {
                var attribute = attributes[a];
                if (!categoryId.HasValue)
                {
                    throw CreateValidationException(
                        $"Products[{i}].Attributes[{a}].ProductAttributeId",
                        "Product attributes require a product category.");
                }

                var definition = await requests.GetProductAttributeAsync(
                    categoryId.Value,
                    attribute.ProductAttributeId,
                    cancellationToken);

                if (definition is null)
                {
                    throw CreateValidationException(
                        $"Products[{i}].Attributes[{a}].ProductAttributeId",
                        "Product attribute does not belong to the selected product category.");
                }

                requestProduct.AddAttribute(attribute.ProductAttributeId, attribute.Value);
            }

            request.AddProduct(requestProduct);
        }

        if (location is not null)
        {
            request.AddLocation(new RequestLocation(
                request.Id,
                location.Province,
                location.City,
                location.District,
                location.Address,
                ToCoordinate(location.Lat),
                ToCoordinate(location.Lng)));
        }

        if (schedule is not null)
        {
            request.AddSchedule(new RequestSchedule(
                request.Id,
                schedule.Date,
                schedule.TimeFrom,
                schedule.TimeTo,
                schedule.IsFlexible));
        }
    }

    private static async Task<long?> ResolveProductCategoryIdAsync(
        RequestProductLineDto line,
        IRequestRepository requests,
        int index,
        CancellationToken cancellationToken)
    {
        if (line.ProductId is null && line.ProductCategoryId is null)
        {
            throw CreateValidationException(
                $"Products[{index}]",
                "A product line must reference a product or a product category.");
        }

        long? categoryId = line.ProductCategoryId;

        if (line.ProductId is not null)
        {
            var product = await requests.GetProductAsync(line.ProductId.Value, cancellationToken);
            if (product is null)
            {
                throw CreateValidationException(
                    $"Products[{index}].ProductId",
                    "The selected product was not found or is inactive.");
            }

            if (categoryId is not null && categoryId.Value != product.CategoryId)
            {
                throw CreateValidationException(
                    $"Products[{index}].ProductCategoryId",
                    "Product category does not match the selected product.");
            }

            categoryId = product.CategoryId;
        }
        else if (categoryId is not null)
        {
            if (!await requests.ProductCategoryExistsAsync(categoryId.Value, cancellationToken))
            {
                throw CreateValidationException(
                    $"Products[{index}].ProductCategoryId",
                    "The selected product category was not found.");
            }
        }

        return categoryId;
    }

    private static decimal? ToCoordinate(double? value) =>
        value is null ? null : (decimal)value.Value;

    private static ValidationException CreateValidationException(string property, string message)
    {
        return new ValidationException(new[]
        {
            new ValidationFailure(property, message)
        });
    }
}
