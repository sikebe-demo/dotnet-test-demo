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
    public string? ErrorMessage { get; private set; }
    public bool IsNotFound { get; private set; }
    public bool IsLoading { get; private set; }

    public async Task<IActionResult> OnGetAsync([FromRoute] string userName)
    {
        if (userName != null)
        {
            _logger.LogInformation("Loading GitHub profile for user: {UserName}", userName);
            IsLoading = true;
            
            var result = await Client.GetUserAsync(userName);
            
            if (result.IsSuccess && result.User != null)
            {
                GitHubUser = result.User;
                _logger.LogInformation("Successfully loaded GitHub profile for user: {UserName}", userName);
            }
            else if (result.IsNotFound)
            {
                IsNotFound = true;
                ErrorMessage = _localizer["UserNotFound"];
                _logger.LogWarning("GitHub user not found: {UserName}", userName);
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? _localizer["UnknownError"];
                _logger.LogError("Error loading GitHub profile for user {UserName}: {Error}", userName, ErrorMessage);
            }
            
            IsLoading = false;
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
