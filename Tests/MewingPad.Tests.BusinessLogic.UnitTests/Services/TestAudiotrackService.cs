using MewingPad.Common.Exceptions;
using MewingPad.Services.AudiotrackService;
using MewingPad.Tests.Factories.Core;
using MewingPad.Utils.AudioManager;

namespace MewingPad.Tests.BusinessLogic.UnitTests.Services;

public class AudiotrackServiceUnitTest : BaseServiceTestClass
{
    private readonly AudiotrackService _audiotrackService;
    private readonly Mock<IAudiotrackRepository> _mockAudiotrackRepository =
        new();
    private readonly Mock<IPlaylistAudiotrackRepository> _mockPlaylistAudiotrackRepository =
        new();
    private readonly Mock<ITagAudiotrackRepository> _mockTagAudiotrackRepository =
        new();
    private readonly Mock<AudioManager> _mockAudioManager = new();

    public AudiotrackServiceUnitTest()
    {
        _audiotrackService = new AudiotrackService(
            _mockAudiotrackRepository.Object,
            _mockPlaylistAudiotrackRepository.Object,
            _mockTagAudiotrackRepository.Object,
            _mockAudioManager.Object
        );
    }

    [Fact]
    public async Task CreateAudiotrackWithStream_AudiotrackUnique_Ok()
    {
        // Arrange
        var expectedTrack = AudiotrackFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1),
            "file.mp3"
        );

        List<Audiotrack> tracks = [];

        Mock<Stream> _mockStream = new();
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(expectedTrack.Id))
            .ReturnsAsync(default(Audiotrack)!);
        _mockAudiotrackRepository
            .Setup(s => s.AddAudiotrack(It.IsAny<Audiotrack>()))
            .Callback((Audiotrack f) => tracks.Add(f));
        _mockAudioManager
            .Setup(s =>
                s.CreateFileFromStreamAsync(
                    _mockStream.Object,
                    expectedTrack.Filepath
                )
            )
            .ReturnsAsync(true);

        // Act
        await _audiotrackService.CreateAudiotrackWithStream(
            _mockStream.Object,
            expectedTrack
        );

        // Assert
        Assert.Single(tracks);
        Assert.Equal(expectedTrack, tracks[0]);
    }

    [Fact]
    public async Task CreateAudiotrackWithStream_UploadServerFailure_Error()
    {
        // Arrange
        var expectedTrack = AudiotrackFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1),
            "file.mp3"
        );

        List<Audiotrack> tracks = [];

        Mock<Stream> _mockStream = new();
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(expectedTrack.Id))
            .ReturnsAsync(default(Audiotrack)!);
        _mockAudiotrackRepository
            .Setup(s => s.AddAudiotrack(It.IsAny<Audiotrack>()))
            .Callback((Audiotrack f) => tracks.Add(f));
        _mockAudiotrackRepository
            .Setup(s => s.DeleteAudiotrack(It.IsAny<Guid>()))
            .Callback((Guid id) => tracks.Remove(expectedTrack));
        _mockAudioManager
            .Setup(s =>
                s.CreateFileFromStreamAsync(
                    _mockStream.Object,
                    expectedTrack.Filepath
                )
            )
            .ReturnsAsync(false);

        // Act
        async Task Action() =>
            await _audiotrackService.CreateAudiotrackWithStream(
                _mockStream.Object,
                expectedTrack
            );

        // Assert
        await Assert.ThrowsAsync<AudiotrackServerUploadException>(Action);
        Assert.Empty(tracks);
    }

    [Fact]
    public async Task CreateAudiotrackWithStream_AudiotrackExists_Error()
    {
        // Arrange
        var expectedTrack = AudiotrackFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1),
            "file.mp3"
        );

        Mock<Stream> _mockStream = new();
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(expectedTrack.Id))
            .ReturnsAsync(expectedTrack);

        // Act
        async Task Action() =>
            await _audiotrackService.CreateAudiotrackWithStream(
                _mockStream.Object,
                expectedTrack
            );

        // Assert
        await Assert.ThrowsAsync<AudiotrackExistsException>(Action);
    }

    [Fact]
    public async Task UpdateAudiotrackWithStream_AudiotrackExists_Ok()
    {
        // Arrange
        var audiotrack = AudiotrackFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1),
            "/path"
        );
        var expectedTrack = new Audiotrack() { Title = "New" };

        var _mockStream = new Mock<Stream>();
        _mockAudioManager
            .Setup(s =>
                s.UpdateFileFromStreamAsync(
                    _mockStream.Object,
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(true);
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(audiotrack);
        _mockAudiotrackRepository
            .Setup(s => s.UpdateAudiotrack(expectedTrack))
            .ReturnsAsync(expectedTrack);

        // Act
        var actual = await _audiotrackService.UpdateAudiotrackWithStream(
            _mockStream.Object,
            expectedTrack
        );

        // Assert
        Assert.Equal(expectedTrack, actual);
    }

    [Fact]
    public async Task UpdateAudiotrackWithStream_UpdateServerFailure_Ok()
    {
        // Arrange
        var expectedTrack = AudiotrackFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1),
            "/path"
        );
        var newTrack = new Audiotrack() { Title = "New" };

        var _mockStream = new Mock<Stream>();
        _mockAudioManager
            .Setup(s =>
                s.UpdateFileFromStreamAsync(
                    _mockStream.Object,
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(false);
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(expectedTrack);
        _mockAudiotrackRepository
            .Setup(s => s.UpdateAudiotrack(It.IsAny<Audiotrack>()))
            .ReturnsAsync(expectedTrack);

        // Act
        async Task Action() =>
            await _audiotrackService.UpdateAudiotrackWithStream(
                _mockStream.Object,
                expectedTrack
            );

        // Assert
        await Assert.ThrowsAsync<AudiotrackServerUploadException>(Action);
        Assert.Equal(3, _mockAudiotrackRepository.Invocations.Count);
    }

    [Fact]
    public async Task UpdateAudiotrackWithStream_AudiotrackNonexistent_Error()
    {
        // Arrange
        var audiotrack = AudiotrackFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1),
            "/path"
        );
        var expectedTrack = new Audiotrack() { Title = "New" };

        var _mockStream = new Mock<Stream>();
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Audiotrack)!);

        // Act
        async Task Action() =>
            await _audiotrackService.UpdateAudiotrackWithStream(
                _mockStream.Object,
                expectedTrack
            );

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    [Fact]
    public async Task DeleteAudiotrack_AudiotrackExists_Ok()
    {
        // Arrange
        List<Audiotrack> tracks =
        [
            AudiotrackFabric.Create(
                MakeGuid(1),
                "Title",
                MakeGuid(1),
                "file.mp3"
            ),
        ];
        var expectedTrack = new Audiotrack(tracks.First());

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(expectedTrack.Id))
            .ReturnsAsync(expectedTrack);
        _mockAudioManager
            .Setup(s => s.DeleteFileAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockAudiotrackRepository
            .Setup(s => s.DeleteAudiotrack(It.IsAny<Guid>()))
            .Callback(
                (Guid id) =>
                {
                    tracks.Remove(expectedTrack);
                }
            );

        // Act
        await _audiotrackService.DeleteAudiotrack(expectedTrack.Id);

        // Assert
        Assert.Empty(tracks);
    }

    [Fact]
    public async Task DeleteAudiotrack_DeleteServerFailure_Error()
    {
        // Arrange
        List<Audiotrack> tracks =
        [
            AudiotrackFabric.Create(
                MakeGuid(1),
                "Title",
                MakeGuid(1),
                "file.mp3"
            ),
        ];
        var expectedTrack = new Audiotrack(tracks.First());

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(expectedTrack.Id))
            .ReturnsAsync(expectedTrack);
        _mockAudioManager
            .Setup(s => s.DeleteFileAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockAudiotrackRepository
            .Setup(s => s.DeleteAudiotrack(expectedTrack.Id))
            .Callback(
                (Guid id) =>
                {
                    tracks.Remove(expectedTrack);
                }
            );

        // Act
        async Task Action() =>
            await _audiotrackService.DeleteAudiotrack(expectedTrack.Id);

        // Assert
        await Assert.ThrowsAsync<AudiotrackServerDeleteException>(Action);
        Assert.Equal(1, _mockAudiotrackRepository.Invocations.Count);
    }

    [Fact]
    public async Task DeleteAudiotrack_AudiotrackNonexistent_Error()
    {
        // Arrange
        List<Audiotrack> tracks =
        [
            AudiotrackFabric.Create(
                MakeGuid(1),
                "Title",
                MakeGuid(1),
                "file.mp3"
            ),
        ];
        var expectedTrack = new Audiotrack(tracks.First());

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(expectedTrack.Id))
            .ReturnsAsync(default(Audiotrack)!);
        _mockAudioManager
            .Setup(s => s.DeleteFileAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        async Task Action() =>
            await _audiotrackService.DeleteAudiotrack(expectedTrack.Id);

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAudiotrackById_AudiotrackExists_ReturnsAudiotrack()
    {
        // Arrange
        var expectedTrack = AudiotrackFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1),
            "file.mp3"
        );

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(expectedTrack.Id))
            .ReturnsAsync(expectedTrack);

        // Act
        var actual = await _audiotrackService.GetAudiotrackById(
            expectedTrack.Id
        );

        // Assert
        Assert.Equal(expectedTrack, actual);
    }

    [Fact]
    public async Task GetAudiotrackById_NoAudiotrackWithId_Error()
    {
        // Arrange

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Audiotrack)!);

        // Act
        async Task Action() =>
            await _audiotrackService.GetAudiotrackById(new Guid());

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAudiotracksByTitle_AudiotracksExist_ReturnsAudiotracks()
    {
        // Arrange
        const string expectedTitle = "Title";
        List<Audiotrack> expectedTracks =
        [
            AudiotrackFabric.Create(
                MakeGuid(1),
                expectedTitle,
                MakeGuid(1),
                "file.mp3"
            ),
        ];

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotracksByTitle(expectedTitle))
            .ReturnsAsync(expectedTracks);

        // Act
        var actual = await _audiotrackService.GetAudiotracksByTitle(
            expectedTitle
        );

        // Assert
        Assert.Equal(expectedTracks, actual);
    }

    [Fact]
    public async Task GetAudiotracksByTitle_NoAudiotracksWithTitle_ReturnsEmpty()
    {
        // Arrange
        const string expectedTitle = "AAA";

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotracksByTitle(expectedTitle))
            .ReturnsAsync([]);

        // Act
        var actual = await _audiotrackService.GetAudiotracksByTitle(
            expectedTitle
        );

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAllAudiotracks_AudiotracksExist_ReturnsAudiotracks()
    {
        // Arrange
        List<Audiotrack> expectedTracks =
        [
            AudiotrackFabric.Create(
                MakeGuid(1),
                "Title",
                MakeGuid(1),
                "file.mp3"
            ),
        ];

        _mockAudiotrackRepository
            .Setup(s => s.GetAllAudiotracks())
            .ReturnsAsync(expectedTracks);

        // Act
        var actual = await _audiotrackService.GetAllAudiotracks();

        // Assert
        Assert.Single(actual);
        Assert.Equal(expectedTracks, actual);
    }

    [Fact]
    public async Task GetAllAudiotracks_NoAudiotracks_ReturnsEmpty()
    {
        // Arrange
        _mockAudiotrackRepository
            .Setup(s => s.GetAllAudiotracks())
            .ReturnsAsync([]);

        // Act
        var actual = await _audiotrackService.GetAllAudiotracks();

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotrackFileStream_SrcpathValid_ReturnsStream()
    {
        // Arrange
        var _mockStream = new Mock<Stream>();
        _mockAudioManager
            .Setup(s => s.GetFileStreamAsync(It.IsAny<string>()))
            .ReturnsAsync(_mockStream.Object);

        // Act
        var actual = await _audiotrackService.GetAudiotrackFileStream("");

        // Assert
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetAudiotrackFileStream_SrcpathInvalid_Error()
    {
        // Arrange
        var _mockStream = new Mock<Stream>();
        _mockAudioManager
            .Setup(s => s.GetFileStreamAsync(It.IsAny<string>()))
            .ReturnsAsync(default(Stream)!);

        // Act
        async Task Action() =>
            await _audiotrackService.GetAudiotrackFileStream("");

        // Assert
        await Assert.ThrowsAsync<AudiotrackServerGetException>(Action);
    }
}
