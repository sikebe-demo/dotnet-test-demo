using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using RazorPagesProject.Data;
using RazorPagesProject.Services;
using RazorPagesProject.Configuration;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configure Features options
builder.Services.Configure<FeaturesOptions>(
    builder.Configuration.GetSection(FeaturesOptions.SectionName));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

var featuresOptions = builder.Configuration.GetSection(FeaturesOptions.SectionName).Get<FeaturesOptions>() ?? new FeaturesOptions();

var razorPagesBuilder = builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/SecurePage");
});

// Conditionally add localization support
if (featuresOptions.EnableMultiLanguageSupport)
{
    razorPagesBuilder.AddViewLocalization().AddDataAnnotationsLocalization();
    
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
}
else
{
    // Add dummy IViewLocalizer and IStringLocalizer services when localization is disabled
    builder.Services.AddSingleton<Microsoft.AspNetCore.Mvc.Localization.IViewLocalizer, EmptyViewLocalizer>();
    builder.Services.AddSingleton(typeof(Microsoft.Extensions.Localization.IStringLocalizer<>), typeof(EmptyStringLocalizer<>));
}

builder.Services.AddHttpClient<IGitHubClient, GitHubClient>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com");
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Yolo", "0.1.0"));
});

builder.Services.AddScoped<IQuoteService, QuoteService>();

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
}

app.UseStaticFiles();

// Conditionally use request localization
var appFeaturesOptions = app.Configuration.GetSection(FeaturesOptions.SectionName).Get<FeaturesOptions>() ?? new FeaturesOptions();
if (appFeaturesOptions.EnableMultiLanguageSupport)
{
    app.UseRequestLocalization();
}

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
