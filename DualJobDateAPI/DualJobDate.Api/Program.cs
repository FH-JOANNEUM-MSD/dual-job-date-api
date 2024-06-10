using System.Security.Claims;
using System.Text;
using AutoMapper;
using DualJobDate.Api.Extensions;
using DualJobDate.Api.Mapping;
using DualJobDate.Api.Middleware;
using DualJobDate.BusinessLogic;
using DualJobDate.BusinessLogic.Helper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.DataAccess;
using DualJobDate.DatabaseInitializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        ServiceRegistration.RegisterServices(services, configuration);
        ConfigureExceptionHandler(services);
        ConfigureCors(services);
        ConfigureDatabase(services, configuration);
        ConfigureIdentity(services);
        ConfigureJwtAuthentication(services, configuration);
        ConfigureAuthorization(services);
        ConfigureSwagger(services);
        ConfigureMapper(services);
        services.AddControllers();
    }

    private static void ConfigureExceptionHandler(IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
#if DEBUG
        var connectionString = configuration.GetConnectionString("AppDebugConnection");
#else
            var connectionString =
                configuration.GetConnectionString("AppReleaseConnection");
#endif
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySQL(connectionString); 
        });
    }

    private static void ConfigureIdentity(IServiceCollection services)
    {
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
#if DEBUG
        string Jwt = "JwtDebug";
#else
        string Jwt = "JwtRelease";
#endif
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var secretKey = configuration[$"{Jwt}:JwtSecret"];
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new InvalidOperationException("JwtSecret is not defined");
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration[$"{Jwt}:Audience"],
                    ValidAudience = configuration[$"{Jwt}:Issuer"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });
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
            options.AddPolicy("AdminOrInstitutionOrStudent", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || context.User.IsInRole("Institution") || context.User.IsInRole("Student")));
            options.AddPolicy("WebApp", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || context.User.IsInRole("Institution") || context.User.IsInRole("Company")));
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

    private static void ConfigureCors(IServiceCollection services)
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
        if (app.Environment.IsDevelopment())
        {
            try
            {
                DbInitializer.InitializeDb(loggerFactory);
            }catch(Exception e)
            {
                var logger = loggerFactory.CreateLogger("DbInitializer");
                logger.LogError(e, "An error occurred while initializing the database.");
            }
        }
        DatabaseConnectionTester.TestDbConnection(app).Wait();
        if (app.Environment.IsDevelopment())
        {
            try
            {
                var dbContext = services.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger("DbInitializer");
                logger.LogError(e, "An error occurred while migrating the database.");
            }
        }
        DbSeeder.SeedData(services).Wait();
        app.UseCors();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        //app.UseSwagger();
        //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DualJobDate API v1"));

        app.MapControllers();
        app.MapGet("/", () => "DualJobDate API.");
    }
}