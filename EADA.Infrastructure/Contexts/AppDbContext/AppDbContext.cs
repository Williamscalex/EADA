using EADA.Core.Contracts.Configuration;
using EADA.Core.Domain.Mapped.Expense;
using Microsoft.EntityFrameworkCore;

namespace EADA.Infrastructure.Contexts;

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

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseType> ExpenseTypes { get; set; }
    public DbSet<ExpenseCategory> ExpensesCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //add schemas and property things
        //EX: builder.Entity<testEntity>(x => {
        //})
        builder.Entity<Expense>(t =>
        {
            t.HasIndex(x => x.ExpenseId);
            t.HasIndex(x => x.ExpenseName).IsUnique();
            t.Property(x => x.ExpenseName).HasMaxLength(50);
            t.Property(x => x.CostPerMonth).HasColumnType("money");
            t.HasOne(x => x.ExpenseType).WithMany().OnDelete(DeleteBehavior.Restrict);
            t.HasOne(x => x.ExpenseCategory).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ExpenseCategory>(t =>
        {
            t.HasIndex(x => x.CategoryName).IsUnique();
            t.Property(x => x.CategoryName).HasMaxLength(50);
            t.Property(x => x.IsSystemDefault).HasDefaultValue(false);
        });
        builder.Entity<ExpenseType>(t =>
        {
            t.HasIndex(x => x.TypeName).IsUnique();
            t.Property(x => x.TypeName).HasMaxLength(50);
            t.Property(x => x.IsSystemDefault).HasDefaultValue(false);
        });
    }

}