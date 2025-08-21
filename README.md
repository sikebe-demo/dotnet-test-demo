# .NET Test Demo

A comprehensive demonstration project showcasing modern .NET development practices, testing strategies, and ASP.NET Core Razor Pages implementation with multiple layers of testing including Unit Tests, Integration Tests, and End-to-End (E2E) Tests.

## 🚀 Project Overview

This solution demonstrates best practices for .NET development and includes the following components:

- **PrimeService**: A mathematical library for prime number validation
- **RazorPagesProject**: An ASP.NET Core Razor Pages web application with Identity framework
- **Multiple Test Projects**: Comprehensive testing coverage with Unit, Integration, and E2E tests

## 📁 Project Structure

```
dotnet-test-demo/
├── src/
│   ├── PrimeService/                    # Core library for prime number calculations
│   ├── PrimeService.Tests/              # Unit tests for PrimeService
│   ├── RazorPagesProject/               # ASP.NET Core Razor Pages web application
│   ├── RazorPagesProject.IntegrationTests/  # Integration tests for web app
│   └── RazorPagesProject.E2ETests/      # End-to-end tests using Selenium
├── coverage/                            # Code coverage reports
├── global.json                          # .NET SDK version specification
└── dotnet-test-demo.sln                # Solution file
```

## 🛠️ Technologies & Frameworks

- **.NET 9** - Latest .NET framework
- **ASP.NET Core** - Web framework
- **Razor Pages** - Page-based web UI framework
- **Entity Framework Core** - Object-relational mapping
- **ASP.NET Core Identity** - Authentication and authorization
- **SQLite** - Database for development and testing
- **Serilog** - Structured logging framework with enrichers
- **xUnit** - Testing framework
- **Selenium WebDriver** - E2E testing automation

## 🏃‍♂️ Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (version 9.0.301 or later)
- A code editor ([Visual Studio](https://visualstudio.microsoft.com/), [Visual Studio Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/))

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/sikebe-demo/dotnet-test-demo.git
   cd dotnet-test-demo
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

### Running the Applications

#### PrimeService Library
The PrimeService is a class library that can be referenced by other projects or tested independently.

#### RazorPagesProject Web Application
```bash
cd src/RazorPagesProject
dotnet run
```

The application will be available at:
- HTTP: http://localhost:5016/
- HTTPS: https://localhost:7072/

**Available Pages:**
- **Home**: http://localhost:5016/
- **GitHub Profile**: http://localhost:5016/GitHubProfile
- **About**: http://localhost:5016/About
- **Contact**: http://localhost:5016/Contact
- **Privacy**: http://localhost:5016/Privacy

## 🧪 Testing

This project demonstrates multiple testing strategies following the testing pyramid approach:

### Unit Tests
Test individual components in isolation:
```bash
dotnet test src/PrimeService.Tests/
```

### Integration Tests
Test the web application with in-memory database:
```bash
dotnet test src/RazorPagesProject.IntegrationTests/
```

### End-to-End Tests
Test complete user scenarios with browser automation:
```bash
dotnet test src/RazorPagesProject.E2ETests/
```

### Run All Tests
```bash
dotnet test
```

### Code Coverage
Generate code coverage reports:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🏗️ Key Features

### PrimeService Library
- Efficient prime number validation algorithm
- Handles edge cases (negative numbers, 0, 1)
- Optimized performance using square root optimization
- Comprehensive unit test coverage

### RazorPagesProject Web Application
- **Authentication & Authorization**: ASP.NET Core Identity integration
- **Structured Logging**: Serilog with request correlation and enrichers
- **Responsive Design**: Mobile-friendly UI
- **Internationalization**: Multi-language support
- **Database Integration**: Entity Framework Core with SQLite
- **Security**: HTTPS, CSRF protection, XSS prevention
- **GitHub Integration**: GitHub profile display functionality

### Logging Features
- **Structured Logging**: JSON-formatted logs with contextual information
- **Request Correlation**: TraceId/RequestId for tracking requests across components
- **Performance Monitoring**: Request timing and response status tracking
- **Environment Enrichment**: Process ID, Thread ID, Environment Name automatically included
- **Exception Tracking**: Detailed error logging with stack traces
- **Configuration-Driven**: Log levels and output format configured in appsettings.json
- **Development-Friendly**: Console output with readable formatting for local development

### Testing Architecture
- **Page Object Pattern**: Maintainable E2E test structure
- **Custom Web Application Factory**: Isolated integration testing
- **Explicit Wait Strategies**: Reliable E2E test execution
- **Parameterized Tests**: Data-driven testing with xUnit Theory

## 🔧 Development Guidelines

### Code Quality Standards
- **C# Latest Features**: Utilizes .NET 9 and C# preview features
- **SOLID Principles**: Clean architecture implementation
- **Async/Await**: Proper asynchronous programming patterns
- **Dependency Injection**: Constructor injection throughout
- **Error Handling**: Comprehensive exception management

### Testing Best Practices
- **AAA Pattern**: Arrange, Act, Assert structure
- **Explicit Waits**: No `Thread.Sleep()` in tests
- **Stable Selectors**: ID-based element selection for E2E tests
- **Test Isolation**: Independent test execution
- **Mock Usage**: Proper isolation in unit tests

## 📊 CI/CD Integration

The project is configured for continuous integration with:
- Automated testing on pull requests
- Code coverage reporting
- Multi-platform testing support

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes following the coding guidelines
4. Add or update tests as necessary
5. Ensure all tests pass (`dotnet test`)
6. Commit your changes (`git commit -m 'Add some amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
