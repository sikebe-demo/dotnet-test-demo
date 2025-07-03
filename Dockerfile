# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0.301 AS build
WORKDIR /src

# Copy project files
COPY ["src/RazorPagesProject/RazorPagesProject.csproj", "src/RazorPagesProject/"]
COPY ["src/PrimeService/PrimeService.csproj", "src/PrimeService/"]
COPY ["Directory.Build.props", "."]
COPY ["global.json", "."]

# Configure NuGet to use insecure connections as a workaround for SSL issues in container environments
ENV NUGET_INSECURE_TLS=true

# Restore dependencies
RUN dotnet restore "src/RazorPagesProject/RazorPagesProject.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/RazorPagesProject"
RUN dotnet build "RazorPagesProject.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "RazorPagesProject.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0.6 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy the published application
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "RazorPagesProject.dll"]