using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DatabaseInitializer
{
    public class DatabaseInitializer(ILogger<DatabaseInitializer> logger)
    {
        public void InitializeDatabaseAsync()
        {
            StartContainer();
        }
    
        private void StartContainer()
        {
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
