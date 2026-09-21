using System;
using System.IO;
using System.Threading.Tasks;
using CatFactsApp.Configuration;
using CatFactsApp.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace CatFactsApp.Tests
{
    public class FactStatisticsServiceTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly FactStatisticsService _sut;

        public FactStatisticsServiceTests()
        {
            _testFilePath = Path.GetTempFileName();
            var options = Options.Create(new AppOptions { OutputFilePath = _testFilePath });
            _sut = new FactStatisticsService(options);
        }

        private async Task WriteMockDataAsync()
        {
            var lines = new[]
            {
                "2023-10-27 12:00:00 UTC - Cat",         
                "2023-10-27 12:01:00 UTC - Cats rule",  
                "2023-10-27 12:02:00 UTC - Feline"      
            };
            await File.WriteAllLinesAsync(_testFilePath, lines);
        }

        [Fact]
        public async Task GetStatistics_ReturnsCorrectTotal()
        {
            await WriteMockDataAsync();
            var stats = await _sut.GetStatisticsAsync();
            Assert.Equal(3, stats.TotalFacts);
        }

        [Fact]
        public async Task GetStatistics_CalculatesAverageLength()
        {
            await WriteMockDataAsync();
            var stats = await _sut.GetStatisticsAsync();
            Assert.Equal(6.0, stats.AverageLength); 
        }

        [Fact]
        public async Task GetStatistics_ReturnsShortestFact()
        {
            await WriteMockDataAsync();
            var stats = await _sut.GetStatisticsAsync();
            Assert.Equal(3, stats.ShortestFactLength);
        }

        [Fact]
        public async Task GetStatistics_ReturnsLongestFact()
        {
            await WriteMockDataAsync();
            var stats = await _sut.GetStatisticsAsync();
            Assert.Equal(9, stats.LongestFactLength);
        }

        [Fact]
        public async Task GetStatistics_HandlesEmptyFile()
        {
            var stats = await _sut.GetStatisticsAsync();

            Assert.Equal(0, stats.TotalFacts);
            Assert.Equal(0, stats.AverageLength);
            Assert.Equal(0, stats.ShortestFactLength);
            Assert.Equal(0, stats.LongestFactLength);
        }

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }
}