using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RazorPagesProject.Services;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesProject.Pages;

public class GitHubProfileModel(IGitHubClient client, IStringLocalizer<GitHubProfileModel> localizer, ILogger<GitHubProfileModel> logger) : PageModel
{
    private readonly IStringLocalizer<GitHubProfileModel> _localizer = localizer;
    private readonly ILogger<GitHubProfileModel> _logger = logger;

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
            try
            {
                GitHubUser = await Client.GetUserAsync(userName);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch GitHub profile for user '{UserName}'. Status: {StatusCode}", 
                    userName, ex.StatusCode);
                ModelState.AddModelError(string.Empty, _localizer["ErrorFetchingProfile"]);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching GitHub profile for user '{UserName}'", 
                    userName);
                ModelState.AddModelError(string.Empty, _localizer["UnexpectedError"]);
                return Page();
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

        if (string.IsNullOrEmpty(Input.UserName))
        {
            ModelState.AddModelError(nameof(Input.UserName), "Username is required.");
            return Page();
        }

        return RedirectToPage(new { Input.UserName });
    }
}
