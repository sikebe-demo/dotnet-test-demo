# dotnet-test-demo

A demonstration .NET 9 project showcasing various testing methodologies, Razor Pages, and automated dependency management.

## Features

- **PrimeService**: A library for prime number calculations
- **RazorPagesProject**: ASP.NET Core Razor Pages web application with localization support
- **Comprehensive Testing**: Unit tests, integration tests, and E2E tests with Playwright
- **Automated Dependency Management**: Dependabot integration for .NET SDK, NuGet packages, and GitHub Actions updates

## Dependabot .NET SDK Management

This project uses Dependabot to automatically manage .NET SDK updates through Docker base images. When new .NET SDK versions are released, Dependabot will automatically create pull requests to update:

- Docker base images (`mcr.microsoft.com/dotnet/sdk` and `mcr.microsoft.com/dotnet/aspnet`)
- NuGet package dependencies
- GitHub Actions versions

### How it works

1. **Dockerfile**: Uses explicit .NET SDK versions (e.g., `mcr.microsoft.com/dotnet/sdk:9.0.301`)
2. **Dependabot**: Monitors Docker ecosystem for base image updates
3. **Automation**: Creates PRs when new .NET versions are available
4. **CI/CD**: Validates changes through automated builds and tests

## Building and Running

### Local Development

```bash
dotnet build
dotnet run --project src/RazorPagesProject
```

### Docker

```bash
# Build and run with Docker
docker build -t dotnet-test-demo .
docker run -p 8080:8080 dotnet-test-demo

# Or use Docker Compose
docker compose up --build
```

## Testing

```bash
# Run all tests
dotnet test

# Run specific test projects
dotnet test src/PrimeService.Tests
dotnet test src/RazorPagesProject.IntegrationTests
dotnet test src/RazorPagesProject.E2ETests
```

## Project Structure

- `src/PrimeService/` - Core business logic library
- `src/PrimeService.Tests/` - Unit tests for PrimeService
- `src/RazorPagesProject/` - ASP.NET Core Razor Pages web application
- `src/RazorPagesProject.IntegrationTests/` - Integration tests
- `src/RazorPagesProject.E2ETests/` - End-to-end tests with Playwright
- `.github/dependabot.yml` - Dependabot configuration for automated updates
