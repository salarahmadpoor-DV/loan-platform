using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence;

/// <summary>
/// Shared SQL Server + migrations assembly setup for runtime DI and EF design-time.
/// Design-time reads the same DefaultConnection as the API (env, user secrets, appsettings).
/// </summary>
public static class MatchiSqlServerConfiguration
{
    public const string ConnectionStringName = "DefaultConnection";
    public const string ApiUserSecretsId = "8f3a1c2e-9b47-4d6a-a1e5-2c8f0d7b6e91";

    public static string MigrationsAssemblyName =>
        typeof(MatchiDbContext).Assembly.GetName().Name
        ?? throw new InvalidOperationException("Matchi.Infrastructure assembly name was not found.");

    public static void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "SQL Server connection string is empty. Set ConnectionStrings:DefaultConnection.");
        }

        options.UseSqlServer(
            connectionString,
            sql => sql.MigrationsAssembly(MigrationsAssemblyName));
    }

    public static string ResolveDesignTimeConnectionString()
    {
        var fromEnv = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (!string.IsNullOrWhiteSpace(fromEnv))
            return fromEnv;

        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Development";

        var fromSecrets = ReadConnectionFromJsonFile(UserSecretsPath());
        if (!string.IsNullOrWhiteSpace(fromSecrets))
            return fromSecrets;

        var apiRoot = LocateApiContentRoot();
        var fromEnvFile = ReadConnectionFromJsonFile(
            Path.Combine(apiRoot, $"appsettings.{environment}.json"));
        if (!string.IsNullOrWhiteSpace(fromEnvFile))
            return fromEnvFile;

        var fromBase = ReadConnectionFromJsonFile(Path.Combine(apiRoot, "appsettings.json"));
        if (!string.IsNullOrWhiteSpace(fromBase))
            return fromBase;

        throw new InvalidOperationException(
            "No usable SQL Server connection string was found for EF design-time. " +
            "Set ConnectionStrings:DefaultConnection via user secrets, " +
            "environment variable ConnectionStrings__DefaultConnection, " +
            "or Matchi.Api/appsettings.*.json. " +
            $"Environment '{environment}'.");
    }

    private static string UserSecretsPath() =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft",
            "UserSecrets",
            ApiUserSecretsId,
            "secrets.json");

    private static string? ReadConnectionFromJsonFile(string path)
    {
        if (!File.Exists(path))
            return null;

        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        var root = doc.RootElement;

        if (root.TryGetProperty("ConnectionStrings", out var nested)
            && nested.ValueKind == JsonValueKind.Object
            && nested.TryGetProperty(ConnectionStringName, out var nestedValue))
        {
            var value = nestedValue.GetString();
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        if (root.TryGetProperty($"ConnectionStrings:{ConnectionStringName}", out var flat))
        {
            var value = flat.GetString();
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }

    private static string LocateApiContentRoot()
    {
        var current = Directory.GetCurrentDirectory();
        var candidates = new[]
        {
            current,
            Path.Combine(current, "Matchi.Api"),
            Path.GetFullPath(Path.Combine(current, "..", "Matchi.Api")),
            Path.GetFullPath(Path.Combine(current, "..", "..", "Matchi.Api"))
        };

        foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (File.Exists(Path.Combine(candidate, "appsettings.json")))
                return candidate;
        }

        throw new InvalidOperationException(
            "Could not locate Matchi.Api/appsettings.json for EF design-time configuration. " +
            "Run the EF CLI from the repository root or Matchi.Api directory.");
    }
}
