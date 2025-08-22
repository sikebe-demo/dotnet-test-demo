using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesProject.Pages;

public class AboutModel : PageModel
{
    public string? Message { get; set; }

    public void OnGet()
    {
        Message = "Your application description page.";
        
        // Set meta description for SEO
        ViewData["MetaDescription"] = "About RazorPagesProject - Learn about our ASP.NET Core web application with localization, GitHub integration, and modern features.";
    }
}
