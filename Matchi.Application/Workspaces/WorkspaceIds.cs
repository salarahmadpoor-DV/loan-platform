namespace Matchi.Application.Workspaces;

public static class WorkspaceIds
{
    public const string Customer = "customer";
    public const string Provider = "provider";
    public const string Business = "business";

    public static readonly string[] FallbackOrder =
    [
        Customer,
        Provider,
        Business
    ];

    public static bool IsKnown(string? workspace)
    {
        return workspace is Customer or Provider or Business;
    }
}
