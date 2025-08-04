using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesProject.Pages;

public class AboutModel : PageModel
{
    public string? Message { get; set; }

    public void OnGet()
    {
        Message = ".NET Test Demo - Comprehensive Testing & Development Showcase";
    }
}
