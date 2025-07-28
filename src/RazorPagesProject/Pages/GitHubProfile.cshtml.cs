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

    public async Task<IActionResult> OnGetAsync([FromRoute] string? userName)
    {
        if (!string.IsNullOrEmpty(userName))
        {
            try
            {
                GitHubUser = await Client.GetUserAsync(userName);
                if (GitHubUser == null)
                {
                    ErrorMessage = _localizer["UserNotFound"];
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = ex.Message.Contains("403") 
                    ? _localizer["ApiLimitExceeded"] 
                    : _localizer["NetworkError"];
            }
            catch (TaskCanceledException)
            {
                ErrorMessage = _localizer["RequestTimeout"];
            }
            catch (Exception)
            {
                ErrorMessage = _localizer["UnexpectedError"];
            }
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Input.UserName))
        {
            ModelState.AddModelError(nameof(Input.UserName), _localizer["UsernameRequired"]);
            return Page();
        }

        // Basic validation for GitHub username format
        if (!IsValidGitHubUsername(Input.UserName))
        {
            ModelState.AddModelError(nameof(Input.UserName), _localizer["InvalidUsernameFormat"]);
            return Page();
        }

        return RedirectToPage(new { userName = Input.UserName });
    }

    private static bool IsValidGitHubUsername(string username)
    {
        // GitHub usernames can only contain alphanumeric characters and hyphens
        // They cannot start or end with a hyphen
        // Maximum length is 39 characters
        if (string.IsNullOrWhiteSpace(username) || username.Length > 39)
            return false;

        if (username.StartsWith('-') || username.EndsWith('-'))
            return false;

        return username.All(c => char.IsLetterOrDigit(c) || c == '-');
    }
}
