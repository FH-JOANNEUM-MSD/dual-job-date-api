# dual-job-date-api

## Starting the Program with Docker (DEV)

This guide outlines the necessary steps to get the application up and running using Docker. This approach simplifies the setup process, ensuring a consistent environment for all developers.

### Prerequisites
Docker must be installed on your system. If it's not already installed, visit the official Docker website for installation instructions.

### Building and Running the Application
Simply start the application in debug mode. A docker-compose file will be executed with `docker-compose up -d`, which creates a MySQL8 database within a docker container and starts it.

### Migrating DB
dotnet ef migrations add Migration_20240311 --project DualJobDate.DataAccess --startup-project DualJobDate.Api

### Updating DB
dotnet ef database update --project DualJobDate.DataAccess --startup-project DualJobDate.Api
