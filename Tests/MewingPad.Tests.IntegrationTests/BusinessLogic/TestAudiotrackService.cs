using MewingPad.Common.Exceptions;
using MewingPad.Database.Context;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Services.AudiotrackService;
using MewingPad.Tests.Factories.Core;
using MewingPad.Utils.AudioManager;
using Moq;

namespace MewingPad.Tests.IntegrationTests.BusinessLogic;

[Collection("Test Database")]
public class TestAudiotrackService : BaseServiceTestClass
{
    private MewingPadDbContext _context;
    private readonly AudiotrackService _service;
    private readonly AudiotrackRepository _audiotrackRepository;
    private readonly PlaylistAudiotrackRepository _playlistAudiotrackRepository;
    private readonly TagAudiotrackRepository _tagAudiotrackRepository;
    private readonly Mock<AudioManager> _mockAudioManager;

    public TestAudiotrackService(DatabaseFixture fixture)
        : base(fixture)
    {
        _context = Fixture.CreateContext();

        _audiotrackRepository = new(_context);
        _playlistAudiotrackRepository = new(_context);
        _tagAudiotrackRepository = new(_context);
        _mockAudioManager = new();

        _service = new(
            _audiotrackRepository,
            _playlistAudiotrackRepository,
            _tagAudiotrackRepository,
            _mockAudioManager.Object
        );
    }

    [Fact]
    public async Task CreateAudiotrackWithStream_AudiotrackUnique_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedTrack = AudiotrackFabric.Create(
            MakeGuid(1),
            "Title",
            DefaultUserId,
            "file.mp3"
        );

        var stream = new MemoryStream();
        _mockAudioManager
            .Setup(s =>
                s.CreateFileFromStreamAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(true);

        // Act
        await _service.CreateAudiotrackWithStream(stream, expectedTrack);

        // Assert
        var actual = (from a in _context.Audiotracks select a).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedTrack.Id, actual[0].Id);
    }

    [Fact]
    public async Task CreateAudiotrackWithStream_UploadServerFailure_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedTrack = AudiotrackFabric.Create(
            MakeGuid(1),
            "Title",
            DefaultUserId,
            "file.mp3"
        );

        var stream = new MemoryStream();
        _mockAudioManager
            .Setup(s =>
                s.CreateFileFromStreamAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(false);

        // Act
        async Task Action() =>
            await _service.CreateAudiotrackWithStream(stream, expectedTrack);

        // Assert
        await Assert.ThrowsAsync<AudiotrackServerUploadException>(Action);
    }

    [Fact]
    public async Task CreateAudiotrackWithStream_AudiotrackExists_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedTrack = AudiotrackFabric.Create(
            DefaultAudiotrackId,
            "Title",
            DefaultUserId,
            "file.mp3"
        );

        var stream = new MemoryStream();

        // Act
        async Task Action() =>
            await _service.CreateAudiotrackWithStream(stream, expectedTrack);

        // Assert
        await Assert.ThrowsAsync<AudiotrackExistsException>(Action);
    }

    [Fact]
    public async Task DeleteAudiotrack_AudiotrackExists_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(1);

        _mockAudioManager
            .Setup(s => s.DeleteFileAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        using (var context = Fixture.CreateContext())
        {
            await context.Audiotracks.AddAsync(
                new AudiotrackDbModelBuilder()
                    .WithId(expectedId)
                    .WithAuthorId(MakeGuid(1))
                    .WithTitle("Title")
                    .WithFilepath("Filepath")
                    .Build()
            );
            await context.SaveChangesAsync();
        }

        // Act
        await _service.DeleteAudiotrack(expectedId);

        // Assert
        var actual = (from a in _context.Audiotracks select a).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateAudiotrackWithStream_AudiotrackExists_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var audiotrack = AudiotrackFabric.Create(
            DefaultAudiotrackId,
            "AAA",
            DefaultUserId,
            "/path"
        );

        _mockAudioManager
            .Setup(s =>
                s.UpdateFileFromStreamAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(true);

        var stream = new MemoryStream();

        // Act
        var actual = await _service.UpdateAudiotrackWithStream(
            stream,
            audiotrack
        );

        // Assert
        Assert.Equal(audiotrack, actual);
    }

    [Fact]
    public async Task UpdateAudiotrackWithStream_UpdateServerFailure_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var audiotrack = AudiotrackFabric.Create(
            DefaultAudiotrackId,
            "AAA",
            DefaultUserId,
            "/path"
        );

        _mockAudioManager
            .Setup(s =>
                s.UpdateFileFromStreamAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(false);

        var stream = new MemoryStream();

        // Act
        async Task Action() =>
            await _service.UpdateAudiotrackWithStream(stream, audiotrack);

        // Assert
        await Assert.ThrowsAsync<AudiotrackServerUploadException>(Action);
    }

    [Fact]
    public async Task UpdateAudiotrackWithStream_AudiotrackNonexistent_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var audiotrack = AudiotrackFabric.Create(
            DefaultAudiotrackId,
            "AAA",
            DefaultUserId,
            "/path"
        );

        var stream = new MemoryStream();

        // Act
        async Task Action() =>
            await _service.UpdateAudiotrackWithStream(stream, audiotrack);

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAudiotrackById_AudiotrackExists_ReturnsAudiotrack()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedTrack = AudiotrackFabric.Create(
            DefaultAudiotrackId,
            "Title",
            DefaultUserId,
            "file.mp3"
        );

        // Act
        var actual = await _service.GetAudiotrackById(expectedTrack.Id);

        // Assert
        Assert.Equal(expectedTrack, actual);
    }

    [Fact]
    public async Task GetAudiotrackById_NoAudiotrackWithId_Error()
    {
        // Arrange

        // Act
        async Task Action() => await _service.GetAudiotrackById(new());

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAudiotracksByTitle_AudiotracksExist_ReturnsAudiotracks()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        const string expectedTitle = "Title";

        // Act
        var actual = await _service.GetAudiotracksByTitle(expectedTitle);

        // Assert
        Assert.Single(actual);
        Assert.Equal(DefaultAudiotrackId, actual[0].Id);
    }

    [Fact]
    public async Task GetAudiotracksByTitle_NoAudiotracksWithTitle_ReturnsEmpty()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        const string expectedTitle = "AAA";

        // Act
        var actual = await _service.GetAudiotracksByTitle(expectedTitle);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAllAudiotracks_AudiotracksExist_ReturnsAudiotracks()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        // Act
        var actual = await _service.GetAllAudiotracks();

        // Assert
        Assert.Single(actual);
    }

    [Fact]
    public async Task GetAllAudiotracks_NoAudiotracks_ReturnsEmpty()
    {
        // Arrange

        // Act
        var actual = await _service.GetAllAudiotracks();

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotrackFileStream_SrcpathValid_ReturnsStream()
    {
        // Arrange
        _mockAudioManager
            .Setup(s => s.GetFileStreamAsync(It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream());

        // Act
        var actual = await _service.GetAudiotrackFileStream("");

        // Assert
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetAudiotrackFileStream_SrcpathInvalid_Error()
    {
        // Arrange
        _mockAudioManager
            .Setup(s => s.GetFileStreamAsync(It.IsAny<string>()))
            .ReturnsAsync(default(MemoryStream)!);

        // Act
        async Task Action() => await _service.GetAudiotrackFileStream("");

        // Assert
        await Assert.ThrowsAsync<AudiotrackServerGetException>(Action);
    }
}
