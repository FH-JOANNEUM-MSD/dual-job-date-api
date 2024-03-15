using System.Reflection;
using AutoMapper;
using DualJobDate.Api.Extensions;
using DualJobDate.BusinessLogic;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using The_Reading_Muse_API.Mapping;

namespace DualJobDate.API
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            RepositoryRegistration.RegisterRepository(builder.Services);
            ServiceRegistration.RegisterServices(builder.Services);
#if DEBUG
            var connectionString =
                builder.Configuration.GetConnectionString("AppDebugConnection");
#else
            var connectionString =
                builder.Configuration.GetConnectionString("AppReleaseConnection");
#endif
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                if (connectionString != null) options.UseMySQL(connectionString);
            });
            
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddControllers();

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                // Identity options configuration
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });

            builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
            builder.Services.AddAuthorizationBuilder();
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DualJobDate API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                    {
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    }
                );
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });
            
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ModelToResourceProfile());
                mc.AddProfile(new ResourceToModelProfile());
            });
            
            var mapper = mappingConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);
            
            var app = builder.Build();

#if DEBUG
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            DatabaseInitializer.DatabaseInitializer.InitializeDb(loggerFactory);
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            DatabaseInitializer.DatabaseInitializer.SeedData(userManager, roleManager);
#endif
            DatabaseConnectionTester.TestDbConnection(app).Wait();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DualJobDate API v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/", () => "DualJobDate API. Following Endpoints are accessible:");
            app.Run();
        }
    }
}
