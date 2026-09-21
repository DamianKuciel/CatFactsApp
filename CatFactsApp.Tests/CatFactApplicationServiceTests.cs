using CatFactsApp.Models;
using CatFactsApp.Services;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace CatFactsApp.Tests
{
    public class CatFactApplicationServiceTests
    {
        private readonly Mock<ICatFactClient> _clientMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly CatFactApplicationService _sut; 

        public CatFactApplicationServiceTests()
        {
            _clientMock = new Mock<ICatFactClient>();
            _fileServiceMock = new Mock<IFileService>();
            _sut = new CatFactApplicationService(_clientMock.Object, _fileServiceMock.Object);
        }

        [Fact]
        public async Task FetchAndSaveAsync_FetchesAndPersistsFact()
        {
            var fakeFact = new CatFactResponse("Cats are cool.", 14);
            _clientMock.Setup(c => c.GetFactAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(fakeFact);

            await _sut.ExecuteAsync();

            _clientMock.Verify(c => c.GetFactAsync(It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(f => f.SaveFactToFileAsync("Cats are cool.", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FetchAndSaveAsync_ReturnsExecutionMetrics()
        {
            var fakeFact = new CatFactResponse("Short fact", 10);
            _clientMock.Setup(c => c.GetFactAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(fakeFact);

            var result = await _sut.ExecuteAsync();

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task FetchAndSaveAsync_DoesNotSaveWhenFactIsEmpty()
        {
            _clientMock.Setup(c => c.GetFactAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync((CatFactResponse?)null);

            var result = await _sut.ExecuteAsync();

            Assert.False(result.IsSuccess);
            _fileServiceMock.Verify(f => f.SaveFactToFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}