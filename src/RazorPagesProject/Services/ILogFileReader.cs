using System.Threading;
using System.Threading.Tasks;

namespace RazorPagesProject.Services;

public interface ILogFileReader
{
    Task<string> ReadAsync(string fileName, CancellationToken cancellationToken = default);
}
