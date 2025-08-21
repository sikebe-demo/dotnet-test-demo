using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using RazorPagesProject.Data;
using RazorPagesProject.Services;
using System.Globalization;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/SecurePage");
    
    // Add filter to set no-cache headers for all Razor Pages
    options.Conventions.ConfigureFilter(new ResponseCacheAttribute
    {
        NoStore = true,
        Location = ResponseCacheLocation.None,
        Duration = 0
    });
})
.AddViewLocalization()
.AddDataAnnotationsLocalization();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ja")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
});

builder.Services.AddHttpClient<IGitHubClient, GitHubClient>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com");
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Yolo", "0.1.0"));
});

builder.Services.AddScoped<IQuoteService, QuoteService>();

// Add response compression (Production only for security)
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    });

    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal;
    });
}

var app = builder.Build();

SeedDatabase(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
    
    // Enable response compression in production
    app.UseResponseCompression();
}

// Configure static files with cache headers
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Set cache headers for versioned assets (files with asp-append-version or fingerprinting)
        var headers = ctx.Context.Response.Headers;
        var path = ctx.Context.Request.Path.Value;
        
        if (path != null)
        {
            // Check if file is versioned (contains query string like ?v=xxxxx)
            if (ctx.Context.Request.Query.ContainsKey("v") || path.Contains("_content/"))
            {
                // Long-lived caching for versioned assets (1 day with immutable)
                headers["Cache-Control"] = "public, max-age=86400, immutable";
            }
            else if (path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".woff") || 
                     path.EndsWith(".woff2") || path.EndsWith(".ttf") || path.EndsWith(".eot"))
            {
                // Short-lived caching for non-versioned static assets
                headers["Cache-Control"] = "public, max-age=3600";
            }
            else
            {
                // Default cache for other static files
                headers["Cache-Control"] = "public, max-age=86400";
            }
        }
    }
});

app.UseRequestLocalization();

app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();

static void SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var container = scope.ServiceProvider;
    var db = container.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!db.Messages.Any())
    {
        try
        {
            db.Initialize();
        }
        catch (Exception ex)
        {
            var logger = container.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
        }
    }
}

public partial class Program { }
