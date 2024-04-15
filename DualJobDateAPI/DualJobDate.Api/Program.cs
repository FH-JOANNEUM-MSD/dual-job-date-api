using AutoMapper;
using DualJobDate.Api.Extensions;
using DualJobDate.Api.Mapping;
using DualJobDate.BusinessLogic;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.DataAccess;
using DualJobDate.DatabaseInitializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace DualJobDate.API;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);
        var app = builder.Build();
        Configure(app);
        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        RepositoryRegistration.RegisterRepository(services);
        ServiceRegistration.RegisterServices(services);
        ConfigureCores(services);
        ConfigureDatabase(services, configuration);
        ConfigureIdentity(services);
        ConfigureJwtAuthentication(services, configuration);
        ConfigureAuthorization(services);
        ConfigureSwagger(services);
        ConfigureMapper(services);
        services.AddControllers();
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
#if DEBUG
        var connectionString = configuration.GetConnectionString("AppDebugConnection");
#else
            var connectionString =
                configuration.GetConnectionString("AppReleaseConnection");
#endif
        services.AddDbContext<AppDbContext>(options => { options.UseMySQL(connectionString); });
    }

    private static void ConfigureIdentity(IServiceCollection services)
    {
        services.AddIdentityApiEndpoints<User>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<Role>()
            .AddEntityFrameworkStores<AppDbContext>();
    }

    private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication();
    }

    private static void ConfigureAuthorization(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => { policy.RequireRole("Admin"); });
            options.AddPolicy("Institution", policy => { policy.RequireRole("Institution"); });
            options.AddPolicy("Student", policy => { policy.RequireRole("Student"); });
            options.AddPolicy("Company", policy => { policy.RequireRole("Company"); });
            options.AddPolicy("AdminOrInstitution", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || context.User.IsInRole("Institution")));
        });
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            option.OperationFilter<SecurityRequirementsOperationFilter>();
        });
    }

    private static void ConfigureMapper(IServiceCollection services)
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ModelToResourceProfile());
            mc.AddProfile(new ResourceToModelProfile());
        });

        var mapper = mappingConfig.CreateMapper();
        services.AddSingleton(mapper);
    }

    private static void ConfigureCores(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    private static void Configure(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        if (app.Environment.IsDevelopment()) DbInitializer.InitializeDb(loggerFactory);
        DatabaseConnectionTester.TestDbConnection(app).Wait();
        DbInitializer.SeedData(services).Wait();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DualJobDate API v1"));

        app.MapControllers();
        app.MapGet("/", () => "DualJobDate API.");
    }
}