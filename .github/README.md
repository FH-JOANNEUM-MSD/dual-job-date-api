# CI/CD Pipeline Documentation

This YAML file defines a Continuous Integration/Continuous Deployment (CI/CD) pipeline for the Dual Job Dating project using Github Actions. The pipeline is triggered on push or pull request events to the `cicd/notification` branch.

## Jobs

This pipeline consists of four main jobs: build, test, build-and-push-docker, and two notification jobs (notify_failure and notify_success).

### Build Job

The build job performs the following steps:

1. **Checkout code**: This step checks out the repository code using the `actions/checkout@v2` action.
2. **Setup .NET Core**: This step sets up the .NET Core environment using the `actions/setup-dotnet@v2` action.
3. **Restore dependencies**: This step restores the project dependencies using the `dotnet restore` command.
4. **Build**: This step builds the project using the `dotnet build` command.
5. **Cache NuGet Packages**: This step caches the NuGet packages to speed up subsequent builds using the `actions/cache@v2` action.

### Test Job

The test job performs the following steps and depends on the completion of the build job:

1. **Checkout code**: This step checks out the repository code using the `actions/checkout@v2` action.
2. **Setup .NET Core**: This step sets up the .NET Core environment using the `actions/setup-dotnet@v2` action.
3. **Run Tests**: This step runs the project tests using the `dotnet test` command.

### Build-and-Push-Docker Job

The build-and-push-docker job performs the following steps and requires permissions to access the Github Container Registry:

1. **Checkout code**: This step checks out the repository code using the `actions/checkout@v2` action.
2. **Convert to lowercase**: This step converts the repository name to lowercase to use it within the image name.
3. **Login to GHCR**: This step logs in to the Github Container Registry using the `docker/login-action@v3` action and the `GITHUB_TOKEN` secret to authenticate.
4. **Build and push Docker image**: This step builds and pushes the container image to the Github Container Registry using the `docker/build-push-action@v5` action.

### Notification Jobs

#### Notify Failure

The notify_failure job sends a notification in case any of the jobs (build, test, build-and-push-docker) fail. It performs the following steps:

1. **Send Discord Notification on Failure**: This step sends a notification to a Discord webhook with details about the workflow failure.

#### Notify Success

The notify_success job sends a notification when all the jobs (build, test, build-and-push-docker) succeed. It performs the following steps:

1. **Send Discord Notification on Success**: This step sends a notification to a Discord webhook with details about the workflow success.

## Permissions

The build-and-push-docker job requires the following permissions:

- **contents: read**
- **packages: write**

## Limitations

- The pipeline is triggered on push or pull request events to the `cicd/notification` branch.
- The pipeline does not automatically deploy the application to the Kubernetes cluster. To deploy the application, you will need to manually deploy the image to the Kubernetes cluster using the `kubectl rollout restart deployment` command.