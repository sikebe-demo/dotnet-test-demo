using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RazorPagesProject.Options;

namespace RazorPagesProject.Services;

public class LogFileReader : ILogFileReader
{
    private readonly IOptions<LogReaderOptions> options;

    public LogFileReader(IOptions<LogReaderOptions> options)
    {
        this.options = options;
    }

    public async Task<string> ReadAsync(string fileName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return string.Empty;
        }

        var directory = options.Value.BaseDirectory;
        Directory.CreateDirectory(directory);

        var fullPath = Path.Combine(directory, fileName);

        if (!File.Exists(fullPath))
        {
            return string.Empty;
        }

        return await File.ReadAllTextAsync(fullPath, cancellationToken);
    }
}
