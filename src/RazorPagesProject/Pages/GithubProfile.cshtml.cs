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

    public async Task<IActionResult> OnGetAsync([FromQuery] string userName)
    {
        if (!string.IsNullOrEmpty(userName))
        {
            try
            {
                GithubUser = await Client.GetUserAsync(userName);
                // 検索後もフォームにユーザー名を保持
                Input = new InputModel { UserName = userName };
            }
            catch (HttpRequestException)
            {
                // 404やAPIエラー時はnullを返し、エラーメッセージをErrorMessageに格納
                GithubUser = null;
                ErrorMessage = $"ユーザー '{userName}' は見つかりませんでした。";
                // エラー時もフォームにユーザー名を保持
                Input = new InputModel { UserName = userName };
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

        // クエリパラメータとしてuserNameを渡す
        return RedirectToPage("/GithubProfile", new { userName = Input.UserName });
    }
}
