using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesProject.Pages;

public class ContactModel : PageModel
{
    public string? Message { get; set; }

    public void OnGet()
    {
        Message = "プロジェクトに関するお問い合わせ";
    }
}
