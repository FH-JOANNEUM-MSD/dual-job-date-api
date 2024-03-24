using DualJobDate.BusinessLogic;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.DataAccess;
using DualJobDate.DatabaseInitializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DualJobDate.Api.Extensions;
using DualJobDate.Api.Mapping;

namespace DualJobDate.API
{
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
                builder.Configuration.GetConnectionString("AppReleaseConnection");
#endif
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySQL(connectionString);
            });
        }

        private static void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>();
        }

        private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSecretKey = configuration["JwtSecret"];
            var bytes = Encoding.UTF8.GetBytes(jwtSecretKey);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(bytes),
                        ValidIssuer = "localhost",
                        ValidAudience = "localhost",
                    };
                });
        }

        private static void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "DualJobDate API", Version = "v1" });
            });
        }

        private static void ConfigureMapper(IServiceCollection services)
        {
            var mappingConfig = new AutoMapper.MapperConfiguration(mc =>
            {
                mc.AddProfile(new ModelToResourceProfile());
                mc.AddProfile(new ResourceToModelProfile());
            });

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private static void Configure(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            DbInitializer.InitializeDb(loggerFactory);
            DatabaseConnectionTester.TestDbConnection(app);
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            DbInitializer.SeedData(userManager, roleManager);

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization(); 

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DualJobDate API v1"));
            }

            app.MapControllers();
            app.MapGet("/", () => "DualJobDate API. Following Endpoints are accessible:");
        }
    }
}
