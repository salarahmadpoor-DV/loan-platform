using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Matchi.Infrastructure.Persistence;

internal sealed class MatchiDbContextFactory : IDesignTimeDbContextFactory<MatchiDbContext>
{
    public MatchiDbContext CreateDbContext(string[] args)
    {
        var connectionString = MatchiSqlServerConfiguration.ResolveDesignTimeConnectionString();
        var options = new DbContextOptionsBuilder<MatchiDbContext>();
        MatchiSqlServerConfiguration.Configure(options, connectionString);
        return new MatchiDbContext(options.Options);
    }
}
