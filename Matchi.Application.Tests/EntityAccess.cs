using System.Reflection;
using Matchi.Domain.Common;

namespace Matchi.Application.Tests;

internal static class EntityAccess
{
    public static T Set<T>(this T entity, string propertyName, object? value)
    {
        var type = entity!.GetType();
        while (type is not null)
        {
            var property = type.GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property is not null)
            {
                property.SetValue(entity, value);
                return entity;
            }

            type = type.BaseType;
        }

        throw new InvalidOperationException($"Property {propertyName} was not found.");
    }

    public static T WithId<T>(this T entity, long id) where T : Entity => entity.Set(nameof(Entity.Id), id);
}
