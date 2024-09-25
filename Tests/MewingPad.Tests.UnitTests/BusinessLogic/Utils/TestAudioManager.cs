using System.Net;
using MewingPad.Utils.AudioManager;
using Microsoft.Extensions.Configuration;
using Moq.Protected;

namespace MewingPad.Tests.UnitTests.BusinessLogic.Utils;

public class TestAudioManager
{
    private Mock<IConfiguration> _mockConfig;

    public TestAudioManager()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockConfig
            .SetupGet(x =>
                x[It.Is<string>(s => s == "ApiSettings:AudioServerAddress")]
            )
            .Returns("http://localhost/");
    }

    [Fact]
    public async Task GetFileStreamAsync_StreamRetrieved_ReturnsStream()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Mock Stream Data"),
                }
            );

        var httpClient = new HttpClient(mockHandler.Object);
        var audioManager = new AudioManager(_mockConfig.Object, httpClient);

        // Act
        var actual = await audioManager.GetFileStreamAsync(
            "https://example.com"
        );

        // Assert
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetFileStreamAsync_ExceptionThrown_ReturnsNull()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback(() => throw new Exception());

        var httpClient = new HttpClient(mockHandler.Object);
        var audioManager = new AudioManager(_mockConfig.Object, httpClient);

        // Act
        var actual = await audioManager.GetFileStreamAsync(
            "https://example.com"
        );

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task CreateFileFromStreamAsync_UploadSuccessful_ReturnsTrue()
    {
        // Arrange
        var fileName = "testfile.mp3";
        var fileStream = new MemoryStream([1, 2, 3, 4, 5]);

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage { StatusCode = HttpStatusCode.OK }
            );

        var httpClient = new HttpClient(mockHandler.Object);
        var audioManager = new AudioManager(_mockConfig.Object, httpClient);

        // Act
        var actual = await audioManager.CreateFileFromStreamAsync(
            fileStream,
            fileName
        );

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async Task CreateFileFromStreamAsync_ExceptionThrown_ReturnsFalse()
    {
        // Arrange
        var fileStream = new MemoryStream([1, 2, 3, 4, 5]);

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback(() => throw new Exception());

        var httpClient = new HttpClient(mockHandler.Object);
        var audioManager = new AudioManager(_mockConfig.Object, httpClient);

        // Act
        var actual = await audioManager.CreateFileFromStreamAsync(
            fileStream,
            ""
        );

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public async Task DeleteFileAsync_DeleteSuccessful_ReturnsTrue()
    {
        // Arrange
        var fileName = "testfile.mp3";

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage { StatusCode = HttpStatusCode.OK }
            );

        var httpClient = new HttpClient(mockHandler.Object);
        var audioManager = new AudioManager(_mockConfig.Object, httpClient);

        // Act
        var actual = await audioManager.DeleteFileAsync(fileName);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async Task DeleteFileAsync_ExceptionThrown_ReturnsFalse()
    {
        // Arrange

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback(() => throw new Exception());

        var httpClient = new HttpClient(mockHandler.Object);
        var audioManager = new AudioManager(_mockConfig.Object, httpClient);

        // Act
        var actual = await audioManager.DeleteFileAsync("");

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public async Task UpdateFileFromStreamAsync_UploadSuccessful_ReturnsTrue()
    {
        // Arrange
        var fileName = "testfile.mp3";
        var fileStream = new MemoryStream([1, 2, 3, 4, 5]);

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage { StatusCode = HttpStatusCode.OK }
            );

        var httpClient = new HttpClient(mockHandler.Object);
        var audioManager = new AudioManager(_mockConfig.Object, httpClient);

        // Act
        var actual = await audioManager.UpdateFileFromStreamAsync(
            fileStream,
            fileName
        );

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async Task UpdateFileFromStreamAsync_ExceptionThrown_ReturnsFalse()
    {
        // Arrange
        var fileStream = new MemoryStream([1, 2, 3, 4, 5]);

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback(() => throw new Exception());

        var httpClient = new HttpClient(mockHandler.Object);
        var audioManager = new AudioManager(_mockConfig.Object, httpClient);

        // Act
        var actual = await audioManager.UpdateFileFromStreamAsync(
            fileStream,
            ""
        );

        // Assert
        Assert.False(actual);
    }
}
