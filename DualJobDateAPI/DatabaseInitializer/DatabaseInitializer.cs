using System.Diagnostics;
using Microsoft.Extensions.Logging;

public class DatabaseInitializer
{
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(ILogger<DatabaseInitializer> logger)
    {
        _logger = logger;
    }

    public void InitializeDatabaseAsync()
    {
        StartContainer();
    }

    private void StartContainer()
    {
        _logger.LogInformation("Starting container using Docker Compose...");
        string workingDirectory = Directory.GetCurrentDirectory();
        _logger.LogInformation(workingDirectory);

        var startInfo = new ProcessStartInfo
        {
            FileName = "docker-compose",
            Arguments = "up -d",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory
        };

        using (var process = Process.Start(startInfo))
        {
            process.WaitForExit();

            // Lies und logge den Standard- und Fehleroutput
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(output))
            {
                _logger.LogInformation($"Output: {output}");
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                _logger.LogInformation($"Output: {error}");
            }
        }
    }
}