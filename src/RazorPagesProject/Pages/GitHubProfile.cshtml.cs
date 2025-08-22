using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RazorPagesProject.Services;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesProject.Pages;

public class GitHubProfileModel(IGitHubClient client, IStringLocalizer<GitHubProfileModel> localizer) : PageModel
{
    private readonly IStringLocalizer<GitHubProfileModel> _localizer = localizer;

    public class InputModel
    {
        [Required]
        public string? UserName { get; set; }
    }

    [BindProperty]
    public required InputModel Input { get; set; }

    public IGitHubClient Client { get; } = client;

    public IStringLocalizer<GitHubProfileModel> Localizer => _localizer;

    public GitHubUser? GitHubUser { get; private set; }

    public async Task<IActionResult> OnGetAsync([FromRoute] string userName)
    {
        if (userName != null)
        {
            GitHubUser = await Client.GetUserAsync(userName);
            // Set dynamic meta description for user profiles
            ViewData["MetaDescription"] = $"GitHub profile for {userName} - View repositories, followers, and contributions on RazorPagesProject.";
        }
        else
        {
            // Set meta description for the search page
            ViewData["MetaDescription"] = "Search GitHub profiles - Enter a username to view detailed GitHub profile information including repositories and followers.";
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (string.IsNullOrEmpty(Input.UserName))
        {
            ModelState.AddModelError(nameof(Input.UserName), "Username is required.");
            return Page();
        }

        return RedirectToPage(new { Input.UserName });
    }
}
