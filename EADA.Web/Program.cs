using EADA.Core.Contracts.Configuration;
using EADA.Infrastructure.AppDbContext;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EADA.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            //seed db
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            IMainConfiguration config = null;
            try
            {
                config = services.GetRequiredService<IMainConfiguration>();
                var dbInitializer = services.GetRequiredService<IDatabaseInitializer>();
                dbInitializer.SeedUsersAsync().Wait();
                dbInitializer.EnsureStoredProcedures().Wait();
            }
            catch (Exception e)
            {
                var ex = new Exception(
                    $"Error during database init. Connection string {config?.ConnectionStrings?.DefaultConnection ?? ""}",
                    e);
                throw new Exception("Unexpected error",ex);
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(options =>
            {
                #if DEBUG
                options.AddUserSecrets(typeof(IMainConfiguration).Assembly);
                options.AddUserSecrets(typeof(Startup).Assembly);
                #endif
            })
            .UseStartup<Startup>();
    }
}