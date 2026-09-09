using Microsoft.Extensions.Options;
using CatFactsApp.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatFactsApp.Services
{
    public interface IFileService
    {
        Task SaveFactToFileAsync(string fact);
    }

    public class FileService : IFileService
    {
        private readonly AppOptions _options;

        public FileService(IOptions<AppOptions> options)
        {
            _options = options.Value;
        }

        public async Task SaveFactToFileAsync(string fact)
        {
            var filePath = _options.OutputFilePath;
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {fact}{Environment.NewLine}";
            await File.AppendAllTextAsync(filePath, line);
        }
    }

}
