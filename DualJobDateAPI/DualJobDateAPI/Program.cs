using DualJobData.BusinessLogic;
using DualJobData.BusinessLogic.Entities;
using DualJobData.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DualJobDate.API
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

            RepositoryRegistration.RegisterRepository(builder.Services);
            ServiceRegistration.RegisterServices(builder.Services);
            string connectionString;

#if DEBUG
            connectionString =
                builder.Configuration.GetConnectionString("AppDebugConnection");
#else
            connectionString =
                builder.Configuration.GetConnectionString("AppReleaseConnection");
#endif
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySQL(connectionString));
            builder.Services.AddSingleton<DatabaseInitializer>();
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

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DualJobDateAPI", Version = "v1" });
            });

            builder.Services.AddControllers();

            var app = builder.Build();

#if DEBUG
            var dbInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
            dbInitializer.InitializeDatabaseAsync();
#endif
            
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    if (context.Database.CanConnect())
                    {
                        logger.LogInformation("Connection to database established!");
                    }
                    else
                    {
                        logger.LogError("Could not establish connection to database!");
                        Environment.Exit(-1);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"Error while trying to establish connection: {ex.Message}");
                    Environment.Exit(-1);
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
