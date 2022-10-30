using System.Reflection;
using EADA.Core.Contracts.Configuration;
using Microsoft.EntityFrameworkCore;

namespace EADA.Infrastructure.AppDbContext;

public partial class AppDbContext :DbContext
{
    private readonly IMainConfiguration _mainConfiguration;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IMainConfiguration mainConfiguration) : base(options)
    {
        _mainConfiguration = mainConfiguration;
    }

    public static Action<DbContextOptionsBuilder> GetOptionsConfigurator(IMainConfiguration mainConfiguration) =>
        builder =>
        {
            builder.UseSqlServer(mainConfiguration.ConnectionStrings.DefaultConnection);
            if (mainConfiguration.EnvironmentName == "Development") builder.EnableSensitiveDataLogging();
        };

    //add dbsets here
    // EX: DbSet<testEntity> TestEntity {get; set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //add schemas and property things
        //EX: builder.Entity<testEntity>(x => {
        //})
    }

}