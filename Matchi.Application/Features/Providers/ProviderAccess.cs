using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Features.Providers;

internal static class Actor
{
    public static long RequireUserId(ICurrentUserService currentUser)
    {
        return currentUser.UserId
            ?? throw new UnauthorizedAccessException("Authenticated user was not found.");
    }
}

internal static class Fail
{
    public static ValidationException Validation(string property, string message) =>
        new(new[] { new ValidationFailure(property, message) });
}

internal static class CatalogRules
{
    public static readonly string[] AreaTypes = ["City", "District", "Province", "Radius"];
    public static readonly string[] MembershipStatuses = ["Active", "Inactive", "Pending"];

    public static bool IsAreaType(string? areaType) =>
        !string.IsNullOrWhiteSpace(areaType)
        && AreaTypes.Contains(areaType, StringComparer.Ordinal);

    public static void EnsureAreaPayload(
        string areaType,
        string? city,
        decimal? lat,
        decimal? lng,
        decimal? radius)
    {
        if (!IsAreaType(areaType))
            throw Fail.Validation("areaType", "Area type must be City, District, Province, or Radius.");

        if (areaType == "Radius" && (lat is null || lng is null || radius is null || radius <= 0))
            throw Fail.Validation("radius", "Radius areas require lat, lng, and a radius greater than zero.");

        if (areaType == "City" && string.IsNullOrWhiteSpace(city))
            throw Fail.Validation("city", "City areas require a city.");
    }
}

internal static class ProviderAccess
{
    public static async Task<Provider> RequireMine(
        ICurrentUserService currentUser,
        IProviderRepository providers,
        CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(currentUser);
        return await providers.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Provider was not found.");
    }
}
