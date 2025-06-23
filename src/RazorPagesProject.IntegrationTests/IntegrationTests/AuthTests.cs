using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using RazorPagesProject.IntegrationTests.Helpers;
using RazorPagesProject.Services;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace RazorPagesProject.IntegrationTests.IntegrationTests;

public class AuthTests :
    IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_GitHubProfilePageCanGetAGitHubUser()
    {
        // Arrange
        static void ConfigureTestServices(IServiceCollection services) =>
            services.AddSingleton<IGitHubClient>(new TestGitHubClient());

        var client = _factory
            .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(ConfigureTestServices))
            .CreateClient();

        // Act
        var profile = await client.GetAsync("/GitHubProfile");
        Assert.Equal(HttpStatusCode.OK, profile.StatusCode);
        var profileHtml = await HtmlHelpers.GetDocumentAsync(profile);

        var profileWithUserName = await client.SendAsync(
            (IHtmlFormElement)profileHtml!.QuerySelector("#user-profile")!,
            new Dictionary<string, string> { ["Input_UserName"] = "user" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, profileWithUserName.StatusCode);
        var profileWithUserHtml = await HtmlHelpers.GetDocumentAsync(profileWithUserName);
        var userLogin = profileWithUserHtml.QuerySelector("#user-login");
        Assert.Equal("user", userLogin?.TextContent);
    }

    public class TestGitHubClient : IGitHubClient
    {
        public Task<GitHubUser?> GetUserAsync(string userName)
        {
            if (userName == "user")
            {
                return Task.FromResult<GitHubUser?>(
                    new GitHubUser
                    {
                        Login = "user",
                        Company = "Contoso Blockchain",
                        Name = "John Doe"
                    });
            }
            else
            {
                return Task.FromResult<GitHubUser?>(null);
            }
        }
    }

    [Fact]
    public async Task Get_SecurePageRedirectsAnUnauthenticatedUser()
    {
        // Arrange
        var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        // Act
        var response = await client.GetAsync("/SecurePage");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("http://localhost/Identity/Account/Login",
            response?.Headers?.Location?.OriginalString);
    }

    [Fact]
    public async Task Get_SecurePageIsReturnedForAnAuthenticatedUser()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "TestScheme", options => { });
            });
        })
        .CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: "TestScheme");

        // Act
        var response = await client.GetAsync("/SecurePage");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
