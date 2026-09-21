using System;
using System.IO;
using System.Threading.Tasks;
using CatFactsApp.Configuration;
using CatFactsApp.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace CatFactsApp.Tests
{
    public class FileServiceTests : IDisposable
    {
        private readonly string _testDirectory;

        public FileServiceTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "CatFactsAppTests_" + Guid.NewGuid().ToString());
        }

        private FileService CreateService(string filePath)
        {
            var options = Options.Create(new AppOptions { OutputFilePath = filePath });
            return new FileService(options);
        }

        [Fact]
        public async Task SaveFactToFileAsync_CreatesFile()
        {
            var filePath = Path.Combine(_testDirectory, "creates_file.txt");
            var service = CreateService(filePath);

            await service.SaveFactToFileAsync("Test fact");

            Assert.True(File.Exists(filePath));
        }

        [Fact]
        public async Task SaveFactToFileAsync_AppendsFacts()
        {
            var filePath = Path.Combine(_testDirectory, "appends_facts.txt");
            var service = CreateService(filePath);

            await service.SaveFactToFileAsync("Fact A");
            await service.SaveFactToFileAsync("Fact B");

            var lines = await File.ReadAllLinesAsync(filePath);

            Assert.Equal(2, lines.Length);
            Assert.Contains("Fact A", lines[0]);
            Assert.Contains("Fact B", lines[1]);
        }

        [Fact]
        public async Task SaveFactToFileAsync_DoesNotOverwriteExistingFacts()
        {
            var filePath = Path.Combine(_testDirectory, "does_not_overwrite.txt");
            Directory.CreateDirectory(_testDirectory);
            await File.WriteAllTextAsync(filePath, "Existing Fact" + Environment.NewLine);

            var service = CreateService(filePath);
            await service.SaveFactToFileAsync("New Fact");

            var lines = await File.ReadAllLinesAsync(filePath);

            Assert.Equal(2, lines.Length);
            Assert.Contains("Existing Fact", lines[0]);
            Assert.Contains("New Fact", lines[1]);
        }

        [Fact]
        public async Task SaveFactToFileAsync_CreatesMissingDirectory()
        {
            var filePath = Path.Combine(_testDirectory, "missing_dir", "nested_dir", "facts.txt");
            var service = CreateService(filePath);

            await service.SaveFactToFileAsync("Directory test");

            Assert.True(Directory.Exists(Path.GetDirectoryName(filePath)));
            Assert.True(File.Exists(filePath));
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}