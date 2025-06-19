using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesProject.Pages;

public class SetLanguageModel : PageModel
{
    public IActionResult OnPost(string culture, string returnUrl)
    {
        if (string.IsNullOrEmpty(culture))
        {
            return LocalRedirect(returnUrl ?? "/");
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false,
                SameSite = SameSiteMode.Lax
            }
        );

        return LocalRedirect(returnUrl ?? "/");
    }
}
