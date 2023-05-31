using Microsoft.AspNetCore.Builder;
using System.Runtime.CompilerServices;
using EADA.Core.ApplicationServices;
using EADA.Core.Contracts;
using EADA.Core.Contracts.ApplicationServices;
using EADA.Core.Contracts.Configuration;
using EADA.Core.Domain.Configuration;
using EADA.Core.Domain.Mapped.Expense;
using EADA.Core.Extensions;
using EADA.Infrastructure;
using EADA.Infrastructure.Contexts;
using EADA.Infrastructure.Repositories;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rbit.FluentValidation.Extensions;
using ServiceCollectionExtensions = EADA.Core.Extensions.ServiceCollectionExtensions;

namespace EADA.Web
{
    public class Startup
    {

        public static Startup Instance { get; }
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;
        private readonly MainConfiguration _mainConfiguration;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _configuration = configuration;

            _mainConfiguration = MainConfiguration.FromConfiguration(configuration, env.EnvironmentName);
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var assembles = (
                core: typeof(IMainConfiguration).Assembly,
                dal: typeof(AppDbContext).Assembly,
                web: typeof(Startup).Assembly);

            services.AddHttpContextAccessor();
            MainConfiguration.RegisterConfiguration(services, _mainConfiguration, true);
            services.AddDbContext<AppDbContext>(AppDbContext.GetOptionsConfigurator(_mainConfiguration));
            services.AddScoped(typeof(DbContext), typeof(AppDbContext));
            services.AddRbitFluentValidationExtensions(assembles.core, assembles.dal, assembles.web);

            //Dynamically register all application services
            services.RegisterSectionByName("ApplicationServices",
                typeof(IMainConfiguration).Assembly,
                ServiceCollectionExtensions.Lifestyle.Scoped);

            //Dynamically register all repositories, excluding the generic IRepository<> interface.
            services.RegisterSectionByName("Repositories", typeof(AppDbContext).Assembly,
                ServiceCollectionExtensions.Lifestyle.Scoped,
                new HashSet<Type>() {typeof(IRepository<>)});


            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IExpenseTypeService, ExpenseTypeService>();
            services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();

            services.AddCors();

            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            services.AddAutoMapper(typeof(MainConfiguration), typeof(Startup));

            //Configurations
            services.Configure<MainConfiguration>(_configuration);

            //Repositories
            services.AddScoped<IUnitOfWork, HttpUnitOfWork>();

            //Db Creation and seeding
            services.AddScoped(typeof(IDatabaseInitializer), typeof(AppDbContext.DatabaseInitializer));

            //TODO fix obsolete fluent validation registration
            services.AddControllers()
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssemblies(new[] { assembles.core, assembles.dal, assembles.web });
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMainConfiguration mainConfiguration)
        {
            if (env.IsDevelopment())
            {
                TelemetryConfiguration.Active.DisableTelemetry = true;
                TelemetryDebugWriter.IsTracingDisabled = true;
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            if (_mainConfiguration.UseHttps) app.UseHttpsRedirection();

            app.UseStaticFiles();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }


            app.UseRouting();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                var ep = context.RequestServices.GetRequiredService<EndpointDataSource>();
                var c = context;
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name:"default",
                    pattern:"{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("https://localhost:4200");
                }
            });
        }
    }
}
