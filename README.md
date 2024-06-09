# Dual-Job-Date-API

## Version 1.0

**The DualJobDate API** provides comprehensive endpoints for managing data related to companies, users, institutions, and academic programs. It is especially useful for educational institutions and companies participating in dual study programs.

### Endpoint Groups

1. **Company Endpoints**
   - **Purpose**: Manage company profiles, update details, retrieve active companies, and manage company-specific information.
   - **Examples**:
      - Retrieve company details
      - Update company information
      - Manage active status of companies
      - Register new companies

2. **User Endpoints**
   - **Purpose**: Handle user registration, authentication, password management, and retrieve specific user data.
   - **Examples**:
      - User registration and authentication
      - Refresh authentication tokens
      - Manage passwords (change and reset)

3. **StudentCompany Endpoints**
   - **Purpose**: Manage interactions between students and companies, like adding or removing likes/dislikes.
   - **Examples**:
      - Add or remove likes/dislikes
      - Retrieve likes/dislikes based on student or company

4. **Util Endpoints**
   - **Purpose**: Provide utilities for managing and retrieving data about institutions and academic programs.
   - **Examples**:
      - Retrieve institutions and academic programs
      - Add or update academic programs

### Authorization and Security

- **OAuth2 with JWT**: Secure access through OAuth2 using JWT tokens. Authentication requires `Authorization: Bearer {token}` header.
- **Roles and Scopes**: Different authorization levels are required such as admin, student, or company-specific roles.

### Important Information

- **Content Types**: Supports `application/json`, `text/json`, and `application/*+json`.
- **Responses**: Uses standard HTTP response codes like `200` (Success), `401` (Unauthorized), and `403` (Forbidden).
- **Versioning**: API versions are marked and should be considered to maintain compatibility.

### Swagger

For detailed API usage and interactive testing, please refer to the [Swagger documentation](https://dual-dating-backend.msd-moss-test.fh-joanneum.at/swagger/index.html).

## Seeded Users in Development Environment

In the development environment, several user profiles are pre-seeded to facilitate testing and interaction with different system roles. Below are the details of these seeded users across various roles:

### Admin User
- **Email**: admin@fh-joanneum.at
- **Password**: Administrator!1
- **Role**: Admin
- **Characteristics**:
   - Linked to a default ADMIN academic program and institution.
   - Has full administrative access to all system functionalities.

### Student Users
- **Student 1**:
   - **Email**: student1@fh-joanneum.at
   - **Password**: Student!1
   - **Role**: Student
- **Student 2**:
   - **Email**: student2@fh-joanneum.at
   - **Password**: Student!1
   - **Role**: Student
- **Characteristics**:
  - Enrolled in the MSD academic program for the year 2021.
  - Registered under the IIT institution.

### Company Users
- **Company 1**:
   - **Email**: company1@fh-joanneum.at
   - **Password**: Company!1
   - **Role**: Company
- **Company 2**:
   - **Email**: company2@fh-joanneum.at
   - **Password**: Company!1
   - **Role**: Company
- **Characteristics**:
   - Linked to the MSD academic program and IIT institution.
   - Designed to simulate user interaction from the perspective of partnered companies.

### Institution User
- **Email**: institution@fh-joanneum.at
- **Password**: Institution!1
- **Role**: Institution
- **Characteristics**:
   - Associated with the IIT institution without specific academic program details and has access to students and companies within the institution.

These profiles help simulate real-world interactions and data management tasks across different user types in the system, enhancing both functionality testing and user interface assessments.

## Starting the Program with Docker
This guide outlines the necessary steps to get the application up and running using Docker. This approach simplifies the setup process, ensuring a consistent environment for all developers.

### Prerequisites
Make sure you have [Docker](https://docker.com), [.NET8](https://learn.microsoft.com/en-gb/dotnet/core/install/) and [EF-Core tools](https://learn.microsoft.com/en-gb/ef/core/cli/dotnet) installed.

### First Steps
1. Clone the [repository](https://github.com/FH-JOANNEUM-MSD/dual-job-date-api.git)
2. Make sure you have [Docker](https://docker.com) installed
3. Navigate to the `DualJobDateAPI` directory
4. Create the docker container (database and API)
    ```
    docker-compose -f docker-compose-dev.yml up -d
    ````
   The application will be started within a docker container in developer mode on [localhost:8000](http://localhost:8000) using another docker container as database on [localhost:3306](http://localhost:3306)
5. Alternatively, run the application in developer mode in an IDE with [.NET8](https://learn.microsoft.com/en-gb/dotnet/core/install/) and [EF-Core tools](https://learn.microsoft.com/en-gb/ef/core/cli/dotnet) installed (Docker still needed for the database and database must be migrated manually). 
6. Open [Swagger](http://localhost:8000/swagger/index.html)

### Migrating DB (Only if there are changes in EF Model)
Create a migration file with the following command:

`dotnet ef migrations add <MigrationName> --project DualJobDate.DataAccess --startup-project DualJobDate.Api`

This only needs to be applied if you change the entity framework model. There are already existing migrations.

### Updating DB
Create or update database scheme with the following command:

`dotnet ef database update --project DualJobDate.DataAccess --startup-project DualJobDate.Api`

## Architecture
For more detailed information on the application architecture, please see [architecture documention](DualJobDateAPI/Architecture.md).