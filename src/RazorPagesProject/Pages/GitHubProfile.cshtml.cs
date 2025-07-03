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
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync([FromRoute] string userName)
    {
        if (userName != null)
        {
            var result = await Client.GetUserAsync(userName);
            if (result.IsSuccess)
            {
                GitHubUser = result.User;
            }
            else if (result.IsUserNotFound)
            {
                ErrorMessage = $"GitHub user '{result.RequestedUserName ?? string.Empty}' not found. Please check the username and try again.";
            }
            else
            {
                ErrorMessage = _localizer["GeneralError"];
            }
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
