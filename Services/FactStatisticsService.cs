using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CatFactsApp.Configuration;
using Microsoft.Extensions.Options;

namespace CatFactsApp.Services
{
    public class FactStatisticsService : IFactStatisticsService
    {
        private readonly AppOptions _options;

        public FactStatisticsService(IOptions<AppOptions> options)
        {
            _options = options.Value;
        }

        public async Task<FactStatistics> GetStatisticsAsync()
        {
            if (!File.Exists(_options.OutputFilePath))
            {
                return new FactStatistics(0, 0, 0, 0);
            }

            var lines = await File.ReadAllLinesAsync(_options.OutputFilePath);
            var factLengths = lines
                .Where(line => line.Contains("UTC - ")) 
                .Select(line => line.Substring(line.IndexOf("UTC - ") + 6).Length)
                .ToList();

            if (!factLengths.Any())
            {
                return new FactStatistics(0, 0, 0, 0);
            }

            return new FactStatistics(
                factLengths.Count,
                factLengths.Average(),
                factLengths.Min(),
                factLengths.Max()
            );
        }
    }
}