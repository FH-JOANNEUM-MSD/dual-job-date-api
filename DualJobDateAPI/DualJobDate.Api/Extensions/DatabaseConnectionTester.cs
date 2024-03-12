using DualJobDate.API;
using DualJobDate.DataAccess;

namespace DualJobDate.Api.Extensions;

public static class DatabaseConnectionTester
{
    public static async Task TestDbConnection(WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        var isConnected = false;
        var attempt = 0;

        while (!isConnected && attempt < 5)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                if (await context.Database.CanConnectAsync())
                {
                    logger.LogInformation("Connection to database established!");
                    isConnected = true;
                    break;
                }
                else
                {
                    logger.LogError("Could not establish connection to database!");
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical("Error while trying to establish connection: {ExMessage}", ex.Message);
            }
        }

        attempt++;
        if (!isConnected && attempt < 5)
        {
            logger.LogInformation("Waiting 5 seconds before attempt {Attempt}...", attempt + 1);
            await Task.Delay(5000);
        }
        

        if (!isConnected)
        {
            logger.LogError("Failed to establish connection after 5 attempts");
            Environment.Exit(-1);
        }
    }
}