
using System.Text;
using Dapper;
using EADA.Core.Contracts.Configuration;
using EADA.Core.Helpers;
using EADA.Infrastructure.Constants;
using EADA.Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace EADA.Infrastructure.AppDbContext;


public interface IDatabaseInitializer
{
    Task SeedUsersAsync();
    Task EnsureStoredProcedures();
}
public partial class AppDbContext
{
    public class DatabaseInitializer : IDatabaseInitializer
    {

        private readonly AppDbContext _context;
        private readonly IMainConfiguration _mainConfiguration;

        public DatabaseInitializer(AppDbContext context, IMainConfiguration mainConfiguration)
        {
            _context = context;
            _mainConfiguration = mainConfiguration;
        }


        public async Task SeedUsersAsync()
        {
            try
            {
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                if (Enumerable.Any(pendingMigrations))
                {
                    var sb = new StringBuilder("Pending migrations found: ");
                    foreach (var pm in pendingMigrations)
                    {
                        sb.AppendLine(pm);
                    }

                    throw new PendingMigrationsException(new Exception(sb.ToString()));
                }

            }
            catch (Exception e)
            {
                throw new DatabaseSeedException(e);
            }
        }

        public async Task EnsureStoredProcedures()
        {
            try
            {
                await using var conn = new SqlConnection(_mainConfiguration.ConnectionStrings.DefaultConnection);
                var missingProcedures = new List<string>();
                var spNames = ReflectionHelper.GetConstantsOfType<string>(typeof(StoredProcedures));
                foreach (var spName in spNames)
                {
                    var result = await conn.QueryFirstOrDefaultAsync<string>(
                        $"select name from sys.procedures where name = @{nameof(spName)}",
                        new { spName });
                    if (string.IsNullOrWhiteSpace(result)) missingProcedures.Add(spName);
                }

                if (missingProcedures.Any())
                    throw new DatabaseInitException("Failed to get missing stored procedures.");
            }
            catch (DatabaseInitException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DatabaseInitException(e);
            }
        }
    }
}