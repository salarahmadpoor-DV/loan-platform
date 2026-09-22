using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Matchi.Infrastructure.Persistence.Seed;

public static class CatalogSeeder
{
    public static async Task SeedAsync(MatchiDbContext context, ILogger logger)
    {
        foreach (var category in CatalogBlueprint.ServiceCategories)
            await EnsureServiceCategoryAsync(context, category, logger);

        var productCategories = new Dictionary<string, ProductCategory>(StringComparer.Ordinal);
        foreach (var category in CatalogBlueprint.ProductCategories.Where(c => c.ParentSlug is null))
        {
            var entity = await EnsureProductCategoryAsync(context, category, null, logger);
            productCategories[category.Slug] = entity;
        }

        foreach (var category in CatalogBlueprint.ProductCategories.Where(c => c.ParentSlug is not null))
        {
            if (!productCategories.TryGetValue(category.ParentSlug!, out var parent))
            {
                logger.LogWarning(
                    "Product category {Slug} skipped because parent {Parent} was not found.",
                    category.Slug,
                    category.ParentSlug);
                continue;
            }

            var entity = await EnsureProductCategoryAsync(context, category, parent.Id, logger);
            productCategories[category.Slug] = entity;
        }

        foreach (var category in CatalogBlueprint.ProductCategories)
        {
            if (!productCategories.TryGetValue(category.Slug, out var entity))
                continue;

            await EnsureProductsAsync(context, entity, category, logger);
            await EnsureProductAttributesAsync(context, entity, category.Attributes, logger);
        }
    }

    private static async Task EnsureServiceCategoryAsync(
        MatchiDbContext context,
        ServiceCategorySeed seed,
        ILogger logger)
    {
        var category = await context.ServiceCategories
            .FirstOrDefaultAsync(c => c.Slug == seed.Slug && !c.IsDeleted);

        if (category is null)
        {
            category = new ServiceCategory(seed.Name, seed.Slug, seed.DisplayOrder);
            await context.ServiceCategories.AddAsync(category);
            await context.SaveChangesAsync();
        }
        else if (!string.Equals(category.Name, seed.Name, StringComparison.Ordinal))
        {
            logger.LogWarning(
                "Service category slug {Slug} already exists as '{Existing}'; seed name '{Seed}' was not applied.",
                seed.Slug,
                category.Name,
                seed.Name);
        }

        for (var i = 0; i < seed.Services.Length; i++)
            await EnsureServiceAsync(context, category.Id, seed.Services[i], i + 1, logger);
    }

    private static async Task EnsureServiceAsync(
        MatchiDbContext context,
        long categoryId,
        ServiceSeed seed,
        int displayOrder,
        ILogger logger)
    {
        var service = await context.Services
            .FirstOrDefaultAsync(s => s.Slug == seed.Slug && !s.IsDeleted);

        if (service is null)
        {
            service = new Service(seed.Name, categoryId, seed.Slug, seed.Description, displayOrder);
            await context.Services.AddAsync(service);
            await context.SaveChangesAsync();
        }
        else
        {
            if (service.CategoryId != categoryId)
            {
                logger.LogWarning(
                    "Service slug {Slug} already exists under another category; seed was not moved.",
                    seed.Slug);
            }
            else if (!string.Equals(service.Name, seed.Name, StringComparison.Ordinal))
            {
                logger.LogWarning(
                    "Service slug {Slug} already exists as '{Existing}'; seed name '{Seed}' was not applied.",
                    seed.Slug,
                    service.Name,
                    seed.Name);
            }
        }

        for (var i = 0; i < seed.Attributes.Length; i++)
            await EnsureServiceAttributeAsync(context, service.Id, seed.Attributes[i], i + 1, logger);
    }

    private static async Task EnsureServiceAttributeAsync(
        MatchiDbContext context,
        long serviceId,
        AttributeSeed seed,
        int displayOrder,
        ILogger logger)
    {
        var attribute = await context.ServiceAttributes
            .FirstOrDefaultAsync(a => a.ServiceId == serviceId && a.Code == seed.Code && !a.IsDeleted);

        if (attribute is null)
        {
            attribute = new ServiceAttribute(serviceId, seed.Name, seed.Code, seed.DataType);
            attribute.Configure(seed.IsRequired, displayOrder);
            await context.ServiceAttributes.AddAsync(attribute);
            await context.SaveChangesAsync();
        }
        else if (!string.Equals(attribute.Name, seed.Name, StringComparison.Ordinal)
                 || !string.Equals(attribute.DataType, seed.DataType, StringComparison.Ordinal))
        {
            logger.LogWarning(
                "Service attribute {Code} on service {ServiceId} already exists with different data; seed was not overwritten.",
                seed.Code,
                serviceId);
        }

        for (var i = 0; i < seed.Options.Length; i++)
            await EnsureServiceOptionAsync(context, attribute.Id, seed.Options[i], i + 1, logger);
    }

    private static async Task EnsureServiceOptionAsync(
        MatchiDbContext context,
        long attributeId,
        OptionSeed seed,
        int displayOrder,
        ILogger logger)
    {
        var option = await context.ServiceAttributeOptions
            .FirstOrDefaultAsync(o => o.ServiceAttributeId == attributeId && o.Value == seed.Value && !o.IsDeleted);

        if (option is null)
        {
            option = new ServiceAttributeOption(attributeId, seed.Value, seed.DisplayName);
            option.SetDisplayOrder(displayOrder);
            await context.ServiceAttributeOptions.AddAsync(option);
            await context.SaveChangesAsync();
            return;
        }

        if (!string.Equals(option.DisplayName, seed.DisplayName, StringComparison.Ordinal))
        {
            logger.LogWarning(
                "Service attribute option {Value} on attribute {AttributeId} already exists as '{Existing}'; seed was not overwritten.",
                seed.Value,
                attributeId,
                option.DisplayName);
        }
    }

    private static async Task<ProductCategory> EnsureProductCategoryAsync(
        MatchiDbContext context,
        ProductCategorySeed seed,
        long? parentId,
        ILogger logger)
    {
        var category = await context.ProductCategories
            .FirstOrDefaultAsync(c => c.Slug == seed.Slug && !c.IsDeleted);

        if (category is null)
        {
            category = new ProductCategory(seed.Name, seed.Slug);
            category.SetHierarchy(parentId, seed.DisplayOrder);
            category.SetDescription(seed.Description);
            await context.ProductCategories.AddAsync(category);
            await context.SaveChangesAsync();
            return category;
        }

        if (!string.Equals(category.Name, seed.Name, StringComparison.Ordinal)
            || category.ParentId != parentId)
        {
            logger.LogWarning(
                "Product category slug {Slug} already exists with different identity; seed was not overwritten.",
                seed.Slug);
        }

        return category;
    }

    private static async Task EnsureProductsAsync(
        MatchiDbContext context,
        ProductCategory category,
        ProductCategorySeed seed,
        ILogger logger)
    {
        foreach (var productSeed in seed.Products)
        {
            var product = await context.Products
                .FirstOrDefaultAsync(p => p.Slug == productSeed.Slug && !p.IsDeleted);

            if (product is null)
            {
                product = new Product(category.Id, productSeed.Name);
                product.SetCatalogDetails(productSeed.Slug, productSeed.Description);
                await context.Products.AddAsync(product);
                await context.SaveChangesAsync();
                continue;
            }

            if (product.CategoryId != category.Id)
            {
                logger.LogWarning(
                    "Product slug {Slug} already exists under another category; seed was not moved.",
                    productSeed.Slug);
            }
            else if (!string.Equals(product.Name, productSeed.Name, StringComparison.Ordinal))
            {
                logger.LogWarning(
                    "Product slug {Slug} already exists as '{Existing}'; seed name '{Seed}' was not applied.",
                    productSeed.Slug,
                    product.Name,
                    productSeed.Name);
            }
        }
    }

    private static async Task EnsureProductAttributesAsync(
        MatchiDbContext context,
        ProductCategory category,
        AttributeSeed[] attributes,
        ILogger logger)
    {
        for (var i = 0; i < attributes.Length; i++)
        {
            var seed = attributes[i];
            var attribute = await context.ProductAttributes
                .FirstOrDefaultAsync(a =>
                    a.ProductCategoryId == category.Id && a.Code == seed.Code && !a.IsDeleted);

            if (attribute is null)
            {
                attribute = new ProductAttribute(category.Id, seed.Name, seed.Code, seed.DataType);
                attribute.Configure(seed.IsRequired, i + 1);
                await context.ProductAttributes.AddAsync(attribute);
                await context.SaveChangesAsync();
            }
            else if (!string.Equals(attribute.Name, seed.Name, StringComparison.Ordinal)
                     || !string.Equals(attribute.DataType, seed.DataType, StringComparison.Ordinal))
            {
                logger.LogWarning(
                    "Product attribute {Code} on category {Slug} already exists with different data; seed was not overwritten.",
                    seed.Code,
                    category.Slug);
            }

            for (var o = 0; o < seed.Options.Length; o++)
            {
                var optionSeed = seed.Options[o];
                var option = await context.ProductAttributeOptions
                    .FirstOrDefaultAsync(x =>
                        x.ProductAttributeId == attribute.Id && x.Value == optionSeed.Value && !x.IsDeleted);

                if (option is null)
                {
                    option = new ProductAttributeOption(attribute.Id, optionSeed.Value, optionSeed.DisplayName);
                    option.SetDisplayOrder(o + 1);
                    await context.ProductAttributeOptions.AddAsync(option);
                    await context.SaveChangesAsync();
                    continue;
                }

                if (!string.Equals(option.DisplayName, optionSeed.DisplayName, StringComparison.Ordinal))
                {
                    logger.LogWarning(
                        "Product attribute option {Value} on attribute {Code} already exists as '{Existing}'; seed was not overwritten.",
                        optionSeed.Value,
                        seed.Code,
                        option.DisplayName);
                }
            }
        }
    }
}
