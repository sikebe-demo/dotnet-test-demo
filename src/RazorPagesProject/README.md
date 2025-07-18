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

This project uses npm to manage client-side packages with cross-platform build support:

- Bootstrap
- jQuery
- jQuery Validation
- jQuery Validation Unobtrusive

### Cross-Platform Build

The project uses a Node.js-based build script (`build-libs.js`) that works on all platforms (Windows, Linux, macOS). The build process runs automatically during `npm install`.

For detailed npm setup documentation, see the dependencies section in this README.

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

Client-side packages are automatically copied to `wwwroot/lib` during `npm install` via the `postinstall` script.

To rebuild client-side assets manually:

```bash
npm run build
```

## References

* https://github.com/dotnet/AspNetCore.Docs.Samples/tree/main/test/integration-tests/IntegrationTestsSample
* https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
