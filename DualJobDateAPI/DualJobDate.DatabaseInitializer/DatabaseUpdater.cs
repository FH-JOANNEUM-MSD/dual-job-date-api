using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DualJobDate.DatabaseInitializer
{
    public static class DatabaseUpdater
    {
        public static void DatabaseMigrationHelper(DbContext dbContext, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("DatabaseMigrationHelper");

            logger.LogInformation("Starting database migration process...");

            var tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempMigrations");
            Directory.CreateDirectory(tempDirectory);

            var migrationName = "AutomaticMigration";
            var migrationFilePath = Path.Combine(tempDirectory, $"{migrationName}.sql");

            var newMigration = dbContext.Database.GenerateCreateScript();

            if (File.Exists(migrationFilePath))
            {
                if (File.ReadAllText(migrationFilePath) == newMigration)
                {
                    logger.LogInformation("Skipping migration due to unchanged database schema.");
                    return;
                }
            }

            File.WriteAllText(migrationFilePath, newMigration);

            try
            {
                DropAllTables(dbContext, logger);
                dbContext.Database.ExecuteSqlRaw(newMigration);
                logger.LogInformation("Migration applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to apply database migration.");
                throw;
            }
        }

        private static void DropAllTables(DbContext dbContext, ILogger logger)
        {
            var connectionString = dbContext.Database.GetConnectionString();
            using var connection = new MySqlConnection(connectionString);

            connection.Open();
            var tableNames = new List<string>();
            var command = connection.CreateCommand();
            command.CommandText = "SHOW TABLES;";
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                tableNames.Add(reader.GetString(0));
            }

            reader.Close();

            foreach (var tableName in tableNames)
            {
                var dropTableCommand = connection.CreateCommand();
                dropTableCommand.CommandText = $@"
                    SET FOREIGN_KEY_CHECKS = 0;
                    DROP TABLE IF EXISTS `{tableName}`;
                    SET FOREIGN_KEY_CHECKS = 1;";
                dropTableCommand.ExecuteNonQuery();
            }

            logger.LogInformation("All tables dropped successfully.");
        }
    }
}
