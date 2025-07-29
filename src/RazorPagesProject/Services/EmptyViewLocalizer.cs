using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace RazorPagesProject.Services;

public class EmptyViewLocalizer : IViewLocalizer
{
    public LocalizedHtmlString this[string name] => new(name, name);

    public LocalizedHtmlString this[string name, params object[] arguments] => new(name, string.Format(name, arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return Enumerable.Empty<LocalizedString>();
    }

    public LocalizedString GetString(string name) => new(name, name);

    public LocalizedString GetString(string name, params object[] arguments) => new(name, string.Format(name, arguments));
}