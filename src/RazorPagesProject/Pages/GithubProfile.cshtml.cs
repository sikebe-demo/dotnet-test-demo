using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RazorPagesProject.Services;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesProject.Pages;

public class GithubProfileModel(IGithubClient client, IStringLocalizer<GithubProfileModel> localizer) : PageModel
{
    private readonly IStringLocalizer<GithubProfileModel> _localizer = localizer;

    public class InputModel
    {
        [Required]
        public string? UserName { get; set; }
    }

    [BindProperty]
    public required InputModel Input { get; set; }

    public IGithubClient Client { get; } = client;

    public IStringLocalizer<GithubProfileModel> Localizer => _localizer;

    public GitHubUser? GithubUser { get; private set; }

    public async Task<IActionResult> OnGetAsync([FromRoute] string userName)
    {
        if (!string.IsNullOrEmpty(userName))
        {
            try
            {
                GithubUser = await Client.GetUserAsync(userName);
                if (GithubUser == null)
                {
                    ModelState.AddModelError(string.Empty, Localizer["UserNotFound", userName]);
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, Localizer["ErrorFetchingUser"]);
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
