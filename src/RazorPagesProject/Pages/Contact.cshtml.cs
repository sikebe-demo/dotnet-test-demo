using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesProject.Pages;

public class ContactModel : PageModel
{
    public string? Message { get; set; }

    public void OnGet()
    {
        Message = "Demo Project - Learning & Collaboration";
    }
}
