# RazorPagesProject

ASP.NET Core Razor Pages application demonstrating modern web development practices.

## Features

* ASP.NET Core 9.0 Razor Pages
* Entity Framework Core with SQLite
* ASP.NET Core Identity for authentication
* Internationalization (i18n) support
* Client-side package management with npm
* Comprehensive testing suite

## Quick Start

### Prerequisites

- .NET 9.0 SDK
- Node.js and npm (for client-side dependencies)

### Setup

1. **Restore .NET packages:**
   ```bash
   dotnet restore
   ```

2. **Install client-side dependencies:**
   ```bash
   cd src/RazorPagesProject
   npm install
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

4. **Access the application:**
   - HTTP: http://localhost:5016
   - HTTPS: https://localhost:7072

## Client-Side Dependencies

This project uses npm to manage client-side packages:

- Bootstrap 5.3.7
- jQuery 3.7.1  
- jQuery Validation 1.21.0
- jQuery Validation Unobtrusive 4.0.0

For detailed npm setup documentation, see [README-NPM.md](README-NPM.md).

## Development

### Building

```bash
dotnet build
```

### Testing

```bash
dotnet test
```

### Client-side assets

Client-side packages are automatically copied to `wwwroot/lib` during npm install. To rebuild manually:

```bash
npm run build
```

## References

* https://github.com/dotnet/AspNetCore.Docs.Samples/tree/main/test/integration-tests/IntegrationTestsSample
* https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
