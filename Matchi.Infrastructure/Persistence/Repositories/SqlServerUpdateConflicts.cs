using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

internal static class SqlServerUpdateConflicts
{
    public static bool IsUniqueIndex(DbUpdateException exception, string indexName)
    {
        var sql = exception.InnerException as SqlException
                  ?? exception.InnerException?.InnerException as SqlException;
        if (sql is null)
            return false;

        if (sql.Number is not (2601 or 2627))
            return false;

        return sql.Message.Contains(indexName, StringComparison.OrdinalIgnoreCase);
    }
}
