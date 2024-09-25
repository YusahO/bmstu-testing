using MewingPad.Common.Exceptions;
using MewingPad.Services.CommentaryService;
using MewingPad.Tests.Factories.Core;

namespace MewingPad.Tests.BusinessLogic.UnitTests.Services;

public class CommentaryServiceUnitTest : BaseServiceTestClass
{
    private readonly CommentaryService _commentaryService;
    private readonly Mock<ICommentaryRepository> _mockCommentaryRepository =
        new();
    private readonly Mock<IAudiotrackRepository> _mockAudiotrackRepository =
        new();

    public CommentaryServiceUnitTest()
    {
        _commentaryService = new CommentaryService(
            _mockCommentaryRepository.Object,
            _mockAudiotrackRepository.Object
        );
    }

    [Fact]
    public async Task CreateCommentary_CommentaryUnique_Ok()
    {
        // Arrange
        var expectedComment = CommentaryFabric.Create(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text"
        );
        List<Commentary> commentaries = [];

        _mockCommentaryRepository
            .Setup(s => s.GetCommentaryById(expectedComment.Id))
            .ReturnsAsync(default(Commentary)!);
        _mockCommentaryRepository
            .Setup(s => s.AddCommentary(It.IsAny<Commentary>()))
            .Callback((Commentary c) => commentaries.Add(c));

        // Act
        await _commentaryService.CreateCommentary(expectedComment);

        // Assert
        Assert.Single(commentaries);
        Assert.Equal(expectedComment, commentaries[0]);
    }

    [Fact]
    public async Task CreateCommentary_CommentaryExists_Error()
    {
        // Arrange
        var expectedComment = CommentaryFabric.Create(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text"
        );
        List<Commentary> commentaries = [];

        _mockCommentaryRepository
            .Setup(s => s.GetCommentaryById(expectedComment.Id))
            .ReturnsAsync(expectedComment);
        _mockCommentaryRepository
            .Setup(s => s.AddCommentary(It.IsAny<Commentary>()))
            .Callback((Commentary c) => commentaries.Add(c));

        // Act
        async Task Action() =>
            await _commentaryService.CreateCommentary(expectedComment);

        // Assert
        await Assert.ThrowsAsync<CommentaryExistsException>(Action);
        Assert.Empty(commentaries);
    }

    [Fact]
    public async Task UpdateCommentary_CommentaryExists_Ok()
    {
        // Arrange
        var oldComment = CommentaryFabric.Create(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text"
        );
        var expectedComment = new Commentary(oldComment) { Text = "New" };

        _mockCommentaryRepository
            .Setup(s => s.GetCommentaryById(expectedComment.Id))
            .ReturnsAsync(expectedComment);
        _mockCommentaryRepository
            .Setup(s => s.UpdateCommentary(oldComment))
            .ReturnsAsync(expectedComment);

        // Act
        var actual = await _commentaryService.UpdateCommentary(
            new(expectedComment)
        );

        // Assert
        Assert.Equal(expectedComment, actual);
    }

    [Fact]
    public async Task UpdateCommentary_CommentaryNonexistent_Error()
    {
        // Arrange
        var expectedComment = CommentaryFabric.Create(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text"
        );

        _mockCommentaryRepository
            .Setup(s => s.GetCommentaryById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Commentary)!);

        // Act
        async Task Action() =>
            await _commentaryService.UpdateCommentary(new(expectedComment));

        // Assert
        await Assert.ThrowsAsync<CommentaryNotFoundException>(Action);
    }

    [Fact]
    public async Task DeleteCommentary_CommentaryExists_Ok()
    {
        // Arrange
        var commentary = CommentaryFabric.Create(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text"
        );
        List<Commentary> commentaries = [commentary];

        _mockCommentaryRepository
            .Setup(s => s.GetCommentaryById(It.IsAny<Guid>()))
            .ReturnsAsync(commentary);
        _mockCommentaryRepository
            .Setup(s => s.DeleteCommentary(commentary.Id))
            .Callback((Guid id) => commentaries.Remove(commentary));

        // Act
        await _commentaryService.DeleteCommentary(commentary.Id);

        // Assert
        Assert.Empty(commentaries);
    }

    [Fact]
    public async Task DeleteCommentary_CommentaryNonexistent_Error()
    {
        // Arrange
        _mockCommentaryRepository
            .Setup(s => s.GetCommentaryById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Commentary)!);

        // Act
        async Task Action() => await _commentaryService.DeleteCommentary(new());

        // Assert
        await Assert.ThrowsAsync<CommentaryNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAudiotrackCommentaries_CommentariesExist_ReturnsCommentaries()
    {
        // Arrange
        var expectedComment = CommentaryFabric.Create(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text"
        );
        List<Commentary> commentaries = [expectedComment];

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(AudiotrackFabric.CreateEmpty());
        _mockCommentaryRepository
            .Setup(s => s.GetAudiotrackCommentaries(It.IsAny<Guid>()))
            .ReturnsAsync(commentaries);

        // Act
        var actual = await _commentaryService.GetAudiotrackCommentaries(new());

        // Assert
        Assert.Single(actual);
    }

    [Fact]
    public async Task GetAudiotrackCommentaries_NoCommentaries_ReturnsEmpty()
    {
        // Arrange
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(AudiotrackFabric.CreateEmpty());
        _mockCommentaryRepository
            .Setup(s => s.GetAudiotrackCommentaries(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // Act
        var actual = await _commentaryService.GetAudiotrackCommentaries(new());

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotrackCommentaries_NoAudiotrackWithId_Error()
    {
        // Arrange
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Audiotrack)!);
        _mockCommentaryRepository
            .Setup(s => s.GetAudiotrackCommentaries(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // Act
        async Task Action() =>
            await _commentaryService.GetAudiotrackCommentaries(new());

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }
}
