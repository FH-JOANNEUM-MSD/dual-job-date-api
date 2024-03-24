using System.Diagnostics;
using DualJobDate.BusinessObjects.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DualJobDate.DatabaseInitializer
{
    public static class DbInitializer
    {
        public static void InitializeDb(ILoggerFactory loggerFactory)
        {

            var logger = loggerFactory.CreateLogger("DbInitializer");
            logger.LogInformation("Starting container using Docker Compose...");

            logger.LogInformation("Starting container using Docker Compose...");
            var workingDirectory = Directory.GetCurrentDirectory();
            var startInfo = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments = "-f db-dev-compose.yml up -d",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            using var process = Process.Start(startInfo);
            process?.WaitForExit();

            var output = process?.StandardOutput.ReadToEnd();
            var error = process?.StandardError.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(output))
            {
                logger.LogInformation("Output: {Output}", output);
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                logger.LogInformation("Output: {Error}", error);
            }
        }
        
        public static async Task SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
        }

        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }
    }
}