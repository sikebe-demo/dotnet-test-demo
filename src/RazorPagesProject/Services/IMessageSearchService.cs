using RazorPagesProject.Data;

namespace RazorPagesProject.Services;

public interface IMessageSearchService
{
    Task<IReadOnlyList<Message>> SearchAsync(string term);
}
