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

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync([FromRoute] string userName)
    {
        if (userName != null)
        {
            try
            {
                GithubUser = await Client.GetUserAsync(userName);
                ErrorMessage = null; // Clear any previous error
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                ErrorMessage = "指定されたユーザーは存在しません";
                GithubUser = null;
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "GitHub APIへの接続中にエラーが発生しました";
                GithubUser = null;
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
