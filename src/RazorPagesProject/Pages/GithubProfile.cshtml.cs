using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesProject.Services;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesProject.Pages;

public class GithubProfileModel : PageModel
{
    public GithubProfileModel(IGithubClient client)
    {
        Client = client;
    }

    public class InputModel
    {
        [Required]
        public string? UserName { get; set; }
    }

    [BindProperty]
    public required InputModel Input { get; set; }

    public IGithubClient Client { get; }

    public GitHubUser? GithubUser { get; private set; }

    public async Task<IActionResult> OnGetAsync([FromRoute] string userName)
    {
        if (userName != null)
        {
            try
            {
                GithubUser = await Client.GetUserAsync(userName);
                if (GithubUser == null)
                {
                    ModelState.AddModelError("", $"ユーザー '{userName}' が見つかりませんでした。");
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "GitHub APIへのアクセス中にエラーが発生しました。");
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
