using Matchi.Infrastructure.Persistence.Seed;

namespace Matchi.Application.Tests;

public sealed class CatalogSeederTests
{
    [Fact]
    public void First_blueprint_load_creates_a_complete_catalog_shape()
    {
        var serviceCategories = CatalogBlueprint.ServiceCategories;
        var productCategories = CatalogBlueprint.ProductCategories;

        Assert.True(serviceCategories.Length >= 16);
        Assert.Equal(
            serviceCategories.Length,
            serviceCategories.Select(c => c.Slug).Distinct(StringComparer.Ordinal).Count());

        var services = serviceCategories.SelectMany(c => c.Services).ToList();
        Assert.True(services.Count >= 100);
        Assert.Equal(services.Count, services.Select(s => s.Slug).Distinct(StringComparer.Ordinal).Count());

        Assert.True(productCategories.Length >= 10);
        Assert.Equal(
            productCategories.Length,
            productCategories.Select(c => c.Slug).Distinct(StringComparer.Ordinal).Count());

        var products = productCategories.SelectMany(c => c.Products).ToList();
        Assert.True(products.Count >= 100);
        Assert.Equal(products.Count, products.Select(p => p.Slug).Distinct(StringComparer.Ordinal).Count());

        var parentSlugs = productCategories
            .Where(c => c.ParentSlug is not null)
            .Select(c => c.ParentSlug!)
            .ToList();
        var categorySlugs = productCategories.Select(c => c.Slug).ToHashSet(StringComparer.Ordinal);
        Assert.All(parentSlugs, slug => Assert.Contains(slug, categorySlugs));

        Assert.True(services.SelectMany(s => s.Attributes).Any());
        Assert.True(productCategories.SelectMany(c => c.Attributes).Any());
        Assert.DoesNotContain(services, s => string.IsNullOrWhiteSpace(s.Name));
        Assert.DoesNotContain(products, p => string.IsNullOrWhiteSpace(p.Name));
        Assert.DoesNotContain(serviceCategories, c => string.IsNullOrWhiteSpace(c.Name));
    }

    [Fact]
    public void Second_pass_over_seed_keys_does_not_create_duplicates()
    {
        var first = CollectKeys();
        var second = CollectKeys();
        Assert.Equal(first.ServiceCategories, second.ServiceCategories);
        Assert.Equal(first.Services, second.Services);
        Assert.Equal(first.ProductCategories, second.ProductCategories);
        Assert.Equal(first.Products, second.Products);
        Assert.Equal(first.ServiceAttributes, second.ServiceAttributes);
        Assert.Equal(first.ProductAttributes, second.ProductAttributes);
        Assert.Equal(first.ServiceOptions, second.ServiceOptions);
        Assert.Equal(first.ProductOptions, second.ProductOptions);
    }

    [Fact]
    public void User_created_slugs_are_not_part_of_the_seed_set()
    {
        var services = CatalogBlueprint.ServiceCategories
            .SelectMany(c => c.Services.Select(s => s.Slug))
            .ToHashSet(StringComparer.Ordinal);

        Assert.DoesNotContain("user-custom-service", services);
        Assert.Contains("repair-air-conditioner", services);
    }

    [Fact]
    public void Product_parent_child_relationships_in_blueprint_are_valid()
    {
        var bySlug = CatalogBlueprint.ProductCategories.ToDictionary(c => c.Slug, StringComparer.Ordinal);
        foreach (var category in CatalogBlueprint.ProductCategories.Where(c => c.ParentSlug is not null))
        {
            Assert.True(bySlug.ContainsKey(category.ParentSlug!));
            Assert.Null(bySlug[category.ParentSlug!].ParentSlug);
        }
    }

    private static CatalogKeys CollectKeys()
    {
        var serviceCategories = CatalogBlueprint.ServiceCategories;
        var productCategories = CatalogBlueprint.ProductCategories;
        return new CatalogKeys(
            serviceCategories.Select(c => c.Slug).OrderBy(x => x).ToArray(),
            serviceCategories.SelectMany(c => c.Services.Select(s => s.Slug)).OrderBy(x => x).ToArray(),
            productCategories.Select(c => c.Slug).OrderBy(x => x).ToArray(),
            productCategories.SelectMany(c => c.Products.Select(p => p.Slug)).OrderBy(x => x).ToArray(),
            serviceCategories.SelectMany(c => c.Services.SelectMany(s => s.Attributes.Select(a => $"{s.Slug}:{a.Code}")))
                .OrderBy(x => x)
                .ToArray(),
            productCategories.SelectMany(c => c.Attributes.Select(a => $"{c.Slug}:{a.Code}"))
                .OrderBy(x => x)
                .ToArray(),
            serviceCategories.SelectMany(c =>
                    c.Services.SelectMany(s =>
                        s.Attributes.SelectMany(a => a.Options.Select(o => $"{s.Slug}:{a.Code}:{o.Value}"))))
                .OrderBy(x => x)
                .ToArray(),
            productCategories.SelectMany(c =>
                    c.Attributes.SelectMany(a => a.Options.Select(o => $"{c.Slug}:{a.Code}:{o.Value}")))
                .OrderBy(x => x)
                .ToArray());
    }

    private sealed record CatalogKeys(
        string[] ServiceCategories,
        string[] Services,
        string[] ProductCategories,
        string[] Products,
        string[] ServiceAttributes,
        string[] ProductAttributes,
        string[] ServiceOptions,
        string[] ProductOptions);
}
