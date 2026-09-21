using Microsoft.Extensions.Options;
using CatFactsApp.Configuration;
using System.Text;

namespace CatFactsApp.Services
{
    public interface IFileService
    {
        Task SaveFactToFileAsync(
            string fact,
            CancellationToken cancellationToken = default);
    }

    public class FileService : IFileService
    {
        private readonly AppOptions _options;

        public FileService(IOptions<AppOptions> options)
        {
            _options = options.Value;
        }

        public async Task SaveFactToFileAsync(
            string fact,
            CancellationToken cancellationToken = default)
        {
            var filePath = _options.OutputFilePath;

            var directory = Path.GetDirectoryName(filePath);

            if(!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var line =
                $"{DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss} UTC - {fact}{Environment.NewLine}";

            await File.AppendAllTextAsync(
                filePath,
                line,
                Encoding.UTF8,
                cancellationToken);

        }
        
    }

}
