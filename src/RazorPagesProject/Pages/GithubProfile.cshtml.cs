using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using RazorPagesProject.Services;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesProject.Pages;

public class GithubProfileModel : PageModel
{
    private readonly IStringLocalizer<GithubProfileModel> _localizer;

    public GithubProfileModel(IGithubClient client, IStringLocalizer<GithubProfileModel> localizer)
    {
        Client = client;
        _localizer = localizer;
    }

    public class InputModel
    {
        [Required]
        public string? UserName { get; set; }
    }

    [BindProperty]
    public required InputModel Input { get; set; }

    public IGithubClient Client { get; }

    public IStringLocalizer<GithubProfileModel> Localizer => _localizer;

    public GitHubUser? GithubUser { get; private set; }

    public async Task<IActionResult> OnGetAsync([FromRoute] string userName)
    {
        if (userName != null)
        {
            GithubUser = await Client.GetUserAsync(userName);
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrEmpty(Input.UserName))
        {
            return Page();
        }

        return RedirectToPage(new { Input.UserName });
    }
}
