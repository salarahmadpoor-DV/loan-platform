namespace Matchi.Application.Workspaces;

public static class WorkspaceResolver
{
    public static IReadOnlyList<string> Available(
        bool hasCustomer,
        bool hasProvider,
        bool hasBusiness)
    {
        var list = new List<string>(3);
        foreach (var id in WorkspaceIds.FallbackOrder)
        {
            var include = id switch
            {
                WorkspaceIds.Customer => hasCustomer,
                WorkspaceIds.Provider => hasProvider,
                WorkspaceIds.Business => hasBusiness,
                _ => false
            };
            if (include)
                list.Add(id);
        }

        return list;
    }

    public static string? Resolve(
        IReadOnlyList<string> available,
        string? preferredWorkspace,
        string? lastWorkspace)
    {
        if (Contains(available, preferredWorkspace))
            return Normalize(preferredWorkspace);

        if (Contains(available, lastWorkspace))
            return Normalize(lastWorkspace);

        return available.Count == 0 ? null : available[0];
    }

    public static bool Contains(IReadOnlyList<string> available, string? workspace)
    {
        var normalized = Normalize(workspace);
        return normalized is not null && available.Contains(normalized, StringComparer.Ordinal);
    }

    public static string? Normalize(string? workspace)
    {
        if (string.IsNullOrWhiteSpace(workspace))
            return null;

        var value = workspace.Trim().ToLowerInvariant();
        return WorkspaceIds.IsKnown(value) ? value : null;
    }
}
