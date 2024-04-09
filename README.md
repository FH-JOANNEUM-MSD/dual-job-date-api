# dual-job-date-api

## Starting the Program with Docker (DEV)

This guide outlines the necessary steps to get the application up and running using Docker. This approach simplifies the setup process, ensuring a consistent environment for all developers.

### Prerequisites
Docker must be installed on your system. If it's not already installed, visit the official Docker website for installation instructions.

## First Steps

### Building and Running the Application
Simply start the application in Rider or Visual Studio. A docker-compose file will be executed with `docker-compose up -d`, which creates a MySQL8 database within a docker container and starts it.
There will be an error after first start since the Database is created only and not migrated. Make sure a container with a MySQL database is created and started. Go on with the next step.

### Migrating DB (Only if there are changes in EF Model)
Create a migration file with the following command:
`dotnet ef migrations add <MigrationName> --project DualJobDate.DataAccess --startup-project DualJobDate.Api`
This only needs to be applied if you change the entity framework model. There is already an existing migration.

### Updating DB
Create database scheme with the following command:
`dotnet ef database update --project DualJobDate.DataAccess --startup-project DualJobDate.Api`

### Starting the application again
After starting the application again, example data will be seeded into the database.
