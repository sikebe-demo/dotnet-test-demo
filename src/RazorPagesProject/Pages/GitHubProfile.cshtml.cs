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
            try
            {
                GitHubUser = await Client.GetUserAsync(userName);
                
                if (GitHubUser == null)
                {
                    ErrorMessage = $"User \"{userName}\" not found. Please check the username and try again.";
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle different types of HTTP errors
                ErrorMessage = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.TooManyRequests => "GitHub API rate limit exceeded. Please try again later.",
                    System.Net.HttpStatusCode.Forbidden => "Access to GitHub API is currently restricted.",
                    _ => "Unable to connect to GitHub API. Please try again later."
                };
            }
            catch (TaskCanceledException)
            {
                ErrorMessage = "Request timed out. Please try again.";
            }
            catch (Exception)
            {
                ErrorMessage = "An unexpected error occurred. Please try again.";
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
