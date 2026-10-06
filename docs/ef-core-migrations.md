# EF Core migrations (Matchi)

Canonical provider: SQL Server. Canonical context: `MatchiDbContext` in `Matchi.Infrastructure`. Canonical schema: `dbo`.

## Where things live

| Piece | Location |
| --- | --- |
| DbContext | `Matchi.Infrastructure/Persistence/MatchiDbContext.cs` |
| Entity configurations | `Matchi.Infrastructure/Persistence/Configurations` |
| Migrations + snapshot | `Matchi.Infrastructure/Persistence/Migrations` |
| Design-time factory | `Matchi.Infrastructure/Persistence/MatchiDbContextFactory.cs` |
| Runtime registration | `Matchi.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs` |
| Startup project | `Matchi.Api` |

Archived files under `Persistence/Migrations/ArchivedIncompatible` are excluded from compilation and are **not** part of the active chain.

## Connection string

Runtime and `dotnet ef` both use `ConnectionStrings:DefaultConnection`.

Resolution order for design-time:

1. Environment variable `ConnectionStrings__DefaultConnection`
2. User secrets on `Matchi.Api` (`UserSecretsId` `8f3a1c2e-9b47-4d6a-a1e5-2c8f0d7b6e91`)
3. `Matchi.Api/appsettings.{Environment}.json` then `appsettings.json`

Set `ASPNETCORE_ENVIRONMENT` (defaults to `Development` for the factory). Do not commit production credentials.

## Create a migration

Run from the repository root:

```powershell
dotnet ef migrations add <MigrationName> `
  --project Matchi.Infrastructure `
  --startup-project Matchi.Api `
  --context MatchiDbContext
```

Review the generated `Up`/`Down`, Designer, and `MatchiDbContextModelSnapshot.cs`. Commit those files with the model change.

## Update the local database

```powershell
dotnet ef database update `
  --project Matchi.Infrastructure `
  --startup-project Matchi.Api `
  --context MatchiDbContext
```

In Development, `Database:MigrateOnStartup` defaults to `true` and `Program.cs` runs `MigrateAsync` before seeding. Production must set that flag explicitly; otherwise startup logs a warning and does **not** apply migrations.

## Inspect pending / applied migrations

```powershell
dotnet ef migrations list `
  --project Matchi.Infrastructure `
  --startup-project Matchi.Api `
  --context MatchiDbContext
```

Against the database:

```sql
SELECT MigrationId, ProductVersion
FROM [dbo].[__EFMigrationsHistory]
ORDER BY MigrationId;
```

History alone does not prove schema correctness. After a migration, verify the tables/columns/indexes it created.

## What not to do

- Do not call `EnsureCreated` / `EnsureDeleted`.
- Do not delete historical migrations or `__EFMigrationsHistory`.
- Do not drop the database as a normal migration fix.
- Do not change SQL by hand without a matching EF migration.
- Do not point design-time at a different database than runtime (no separate `MatchiDesign` LocalDB).

Every entity/configuration schema change needs a migration, and those files ship with the code change.
