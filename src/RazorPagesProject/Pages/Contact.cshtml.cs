using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesProject.Pages;

public class ContactModel : PageModel
{
    public string? Message { get; set; }

    public void OnGet()
    {
        Message = "Get in touch with the development team.";
    }
}
