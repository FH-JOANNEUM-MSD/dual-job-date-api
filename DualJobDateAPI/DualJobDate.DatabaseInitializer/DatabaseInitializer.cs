using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DualJobDate.DatabaseInitializer
{
    public static class DatabaseInitializer
    {
        public static void InitializeDb(ILoggerFactory loggerFactory)
        {

            var logger = loggerFactory.CreateLogger("DatabaseInitializer");
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
    }
}