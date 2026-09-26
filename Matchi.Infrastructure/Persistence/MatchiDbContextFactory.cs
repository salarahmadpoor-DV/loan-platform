using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Matchi.Infrastructure.Persistence;

internal sealed class MatchiDbContextFactory : IDesignTimeDbContextFactory<MatchiDbContext>
{
    public MatchiDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MatchiDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MatchiDesign;Trusted_Connection=True;")
            .Options;

        return new MatchiDbContext(options);
    }
}
