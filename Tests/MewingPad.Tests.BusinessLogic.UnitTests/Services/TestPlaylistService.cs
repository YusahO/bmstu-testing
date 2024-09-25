using MewingPad.Common.Exceptions;
using MewingPad.Services.PlaylistService;
using MewingPad.Tests.Factories.Core;

namespace MewingPad.Tests.BusinessLogic.UnitTests.Services;

public class PlaylistServiceUnitTest : BaseServiceTestClass
{
    private readonly PlaylistService _playlistService;
    private readonly Mock<IPlaylistRepository> _mockPlaylistRepository = new();
    private readonly Mock<IAudiotrackRepository> _mockAudiotrackRepository =
        new();
    private readonly Mock<IPlaylistAudiotrackRepository> _mockPlaylistAudiotrackRepository =
        new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();

    public PlaylistServiceUnitTest()
    {
        _playlistService = new PlaylistService(
            _mockPlaylistRepository.Object,
            _mockAudiotrackRepository.Object,
            _mockUserRepository.Object,
            _mockPlaylistAudiotrackRepository.Object
        );
    }

    [Fact]
    public async Task CreatePlaylist_PlaylistUnique_Ok()
    {
        // Arrange
        var expectedPlaylist = PlaylistFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1)
        );
        List<Playlist> playlists = [];

        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Playlist)!);
        _mockPlaylistRepository
            .Setup(s => s.AddPlaylist(It.IsAny<Playlist>()))
            .Callback((Playlist p) => playlists.Add(expectedPlaylist));

        // Act
        await _playlistService.CreatePlaylist(new(expectedPlaylist));

        // Assert
        Assert.Single(playlists);
        Assert.Equal(expectedPlaylist, playlists[0]);
    }

    [Fact]
    public async Task CreatePlaylist_PlaylistExists_Error()
    {
        // Arrange
        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(PlaylistFabric.CreateEmpty());

        // Act
        async Task Action() =>
            await _playlistService.CreatePlaylist(PlaylistFabric.CreateEmpty());

        // Assert
        await Assert.ThrowsAsync<PlaylistExistsException>(Action);
    }

    [Fact]
    public async Task UpdatePlaylistTitle_PlaylistExists_Ok()
    {
        // Arrange
        var oldPlaylist = PlaylistFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1)
        );
        Playlist expectedPlaylist = new(oldPlaylist) { Title = "New" };

        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(oldPlaylist);
        _mockPlaylistRepository
            .Setup(s => s.UpdatePlaylist(It.IsAny<Playlist>()))
            .ReturnsAsync(expectedPlaylist);

        // Act
        var actual = await _playlistService.UpdatePlaylistTitle(
            expectedPlaylist.Id,
            expectedPlaylist.Title
        );

        // Assert
        Assert.Equal(expectedPlaylist, actual);
    }

    [Fact]
    public async Task UpdatePlaylistTitle_PlaylistNonexistent_Error()
    {
        // Arrange
        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Playlist)!);

        // Act
        async Task Action() =>
            await _playlistService.UpdatePlaylistTitle(new Guid(), "");

        // Assert
        await Assert.ThrowsAsync<PlaylistNotFoundException>(Action);
    }

    [Fact]
    public async Task DeletePlaylist_PlaylistExists_Ok()
    {
        // Arrange
        var playlist = PlaylistFabric.Create(MakeGuid(1), "Title", MakeGuid(1));
        List<Playlist> playlists = [playlist];

        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(playlist);
        _mockPlaylistRepository
            .Setup(s => s.DeletePlaylist(It.IsAny<Guid>()))
            .Callback((Guid id) => playlists.Remove(playlist));

        // Act
        await _playlistService.DeletePlaylist(playlist.Id);

        // Assert
        Assert.Empty(playlists);
    }

    [Fact]
    public async Task DeletePlaylist_PlaylistNonexistent_Error()
    {
        // Arrange
        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Playlist)!);

        // Act
        async Task Action() => await _playlistService.DeletePlaylist(new());

        // Assert
        await Assert.ThrowsAsync<PlaylistNotFoundException>(Action);
    }

    [Fact]
    public async Task AddAudiotrackToPlaylist_AddUnique_Ok()
    {
        // Arrange
        var expectedPlaylistId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);
        List<Tuple<Guid, Guid>> playlistsAudiotracks = [];

        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(PlaylistFabric.CreateEmpty());
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(AudiotrackFabric.CreateEmpty());
        _mockPlaylistAudiotrackRepository
            .Setup(s =>
                s.IsAudiotrackInPlaylist(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(false);
        _mockPlaylistAudiotrackRepository
            .Setup(s =>
                s.AddAudiotrackToPlaylist(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .Callback(
                (Guid pid, Guid aid) =>
                    playlistsAudiotracks.Add(
                        Tuple.Create(expectedPlaylistId, expectedAudiotrackId)
                    )
            );

        // Act
        await _playlistService.AddAudiotrackToPlaylist(
            expectedPlaylistId,
            expectedAudiotrackId
        );

        // Assert
        Assert.Single(playlistsAudiotracks);
    }

    public static IEnumerable<object[]> AddAudiotrackToPlaylist_GetTestData()
    {
        yield return new object[]
        {
            default(Playlist)!,
            AudiotrackFabric.CreateEmpty(),
            false,
            typeof(PlaylistNotFoundException),
        };
        yield return new object[]
        {
            PlaylistFabric.CreateEmpty(),
            default(Audiotrack)!,
            false,
            typeof(AudiotrackNotFoundException),
        };
        yield return new object[]
        {
            PlaylistFabric.CreateEmpty(),
            AudiotrackFabric.CreateEmpty(),
            true,
            typeof(AudiotrackExistsInPlaylistException),
        };
    }

    [Theory]
    [MemberData(nameof(AddAudiotrackToPlaylist_GetTestData))]
    public async Task AddAudiotrackToPlaylist_Error(
        Playlist returnedPlaylist,
        Audiotrack returnedAudiotrack,
        bool isAudiotrackInPlaylist,
        Type expectedExceptionType
    )
    {
        // Arrange
        var expectedPlaylistId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);
        List<Tuple<Guid, Guid>> playlistsAudiotracks = [];

        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedPlaylist);
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedAudiotrack);
        _mockPlaylistAudiotrackRepository
            .Setup(s =>
                s.IsAudiotrackInPlaylist(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(isAudiotrackInPlaylist);
        _mockPlaylistAudiotrackRepository
            .Setup(s =>
                s.AddAudiotrackToPlaylist(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .Callback(
                (Guid pid, Guid aid) =>
                    playlistsAudiotracks.Add(
                        new(expectedPlaylistId, expectedAudiotrackId)
                    )
            );

        // Act
        var exception = await Assert.ThrowsAsync(
            expectedExceptionType,
            async () =>
            {
                await _playlistService.AddAudiotrackToPlaylist(new(), new());
            }
        );

        // Assert
        Assert.IsType(expectedExceptionType, exception);
    }

    [Fact]
    public async Task RemoveAudiotrackFromPlaylist_DeleteExisting_Ok()
    {
        // Arrange
        var expectedPlaylistId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);
        List<Tuple<Guid, Guid>> playlistsAudiotracks =
        [
            Tuple.Create(expectedPlaylistId, expectedAudiotrackId),
        ];

        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(PlaylistFabric.CreateEmpty());
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(AudiotrackFabric.CreateEmpty());
        _mockPlaylistAudiotrackRepository
            .Setup(s =>
                s.IsAudiotrackInPlaylist(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(true);
        _mockPlaylistAudiotrackRepository
            .Setup(s =>
                s.RemoveAudiotrackFromPlaylist(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>()
                )
            )
            .Callback(
                (Guid pid, Guid aid) =>
                    playlistsAudiotracks.Remove(
                        Tuple.Create(expectedPlaylistId, expectedAudiotrackId)
                    )
            );

        // Act
        await _playlistService.RemoveAudiotrackFromPlaylist(
            expectedPlaylistId,
            expectedAudiotrackId
        );

        // Assert
        Assert.Empty(playlistsAudiotracks);
    }

    public static IEnumerable<object[]> RemoveAudiotrackFromPlaylist_GetTestData()
    {
        yield return new object[]
        {
            default(Playlist)!,
            AudiotrackFabric.CreateEmpty(),
            true,
            typeof(PlaylistNotFoundException),
        };
        yield return new object[]
        {
            PlaylistFabric.CreateEmpty(),
            default(Audiotrack)!,
            true,
            typeof(AudiotrackNotFoundException),
        };
        yield return new object[]
        {
            PlaylistFabric.CreateEmpty(),
            AudiotrackFabric.CreateEmpty(),
            false,
            typeof(AudiotrackNotFoundInPlaylistException),
        };
    }

    [Theory]
    [MemberData(nameof(RemoveAudiotrackFromPlaylist_GetTestData))]
    public async Task RemoveAudiotrackFromPlaylist_Error(
        Playlist returnedPlaylist,
        Audiotrack returnedAudiotrack,
        bool isAudiotrackInPlaylist,
        Type expectedExceptionType
    )
    {
        // Arrange
        var expectedPlaylistId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);
        List<Tuple<Guid, Guid>> playlistsAudiotracks =
        [
            Tuple.Create(expectedPlaylistId, expectedAudiotrackId),
        ];

        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedPlaylist);
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedAudiotrack);
        _mockPlaylistAudiotrackRepository
            .Setup(s =>
                s.IsAudiotrackInPlaylist(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(isAudiotrackInPlaylist);
        _mockPlaylistAudiotrackRepository
            .Setup(s =>
                s.RemoveAudiotrackFromPlaylist(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>()
                )
            )
            .Callback(
                (Guid pid, Guid aid) =>
                    playlistsAudiotracks.Remove(
                        new(expectedPlaylistId, expectedAudiotrackId)
                    )
            );

        // Act
        var exception = await Assert.ThrowsAsync(
            expectedExceptionType,
            async () =>
            {
                await _playlistService.RemoveAudiotrackFromPlaylist(
                    new(),
                    new()
                );
            }
        );

        // Assert
        Assert.IsType(expectedExceptionType, exception);
    }

    [Fact]
    public async Task GetUserFavouritesPlaylist_UserAndPlaylistExist_ReturnsPlaylist()
    {
        // Arrange
        var expectedPlaylist = PlaylistFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1)
        );

        _mockUserRepository
            .Setup(s => s.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(UserFabric.CreateEmpty());
        _mockPlaylistRepository
            .Setup(s => s.GetUserFavouritesPlaylist(It.IsAny<Guid>()))
            .ReturnsAsync(new Playlist(expectedPlaylist));

        // Act
        var actual = await _playlistService.GetUserFavouritesPlaylist(new());

        // Assert
        Assert.Equal(expectedPlaylist, actual);
    }

    public static IEnumerable<object[]> GetUserFavouritesPlaylist_GetTestData()
    {
        yield return new object[]
        {
            default(User)!,
            PlaylistFabric.CreateEmpty(),
            typeof(UserNotFoundException),
        };
        yield return new object[]
        {
            UserFabric.CreateEmpty(),
            default(Playlist)!,
            typeof(PlaylistNotFoundException),
        };
    }

    [Theory]
    [MemberData(nameof(GetUserFavouritesPlaylist_GetTestData))]
    public async Task GetUserFavouritesPlaylist_Error(
        User returnedUser,
        Playlist returnedPlaylist,
        Type expectedExceptionType
    )
    {
        // Arrange
        var expectedPlaylist = PlaylistFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1)
        );

        _mockUserRepository
            .Setup(s => s.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedUser);
        _mockPlaylistRepository
            .Setup(s => s.GetUserFavouritesPlaylist(It.IsAny<Guid>()))
            .ReturnsAsync(returnedPlaylist);

        // Act
        var exception = await Assert.ThrowsAsync(
            expectedExceptionType,
            async () =>
            {
                await _playlistService.GetUserFavouritesPlaylist(new());
            }
        );

        // Assert
        Assert.IsType(expectedExceptionType, exception);
    }

    public static IEnumerable<object[]> GetAllAudiotracksFromPlaylist_GetTestData()
    {
        yield return new object[] { new List<Audiotrack>() };
        yield return new object[]
        {
            new List<Audiotrack>([AudiotrackFabric.CreateEmpty()]),
        };
    }

    [Theory]
    [MemberData(nameof(GetAllAudiotracksFromPlaylist_GetTestData))]
    public async Task GetAllAudiotracksFromPlaylist_ReturnsFoundAudiotracks(
        List<Audiotrack> expectedAudiotracks
    )
    {
        // Arrange
        var expectedPlaylist = PlaylistFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1)
        );

        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(expectedPlaylist);
        _mockPlaylistAudiotrackRepository
            .Setup(s => s.GetAllAudiotracksFromPlaylist(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Audiotrack>(expectedAudiotracks));

        // Act
        var actual = await _playlistService.GetAllAudiotracksFromPlaylist(
            new()
        );

        // Assert
        Assert.Equal(expectedAudiotracks, actual);
    }

    [Fact]
    public async Task GetAllAudiotracksFromPlaylist_NoPlaylistWitId_Error()
    {
        // Arrange
        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Playlist)!);

        // Act
        async Task Action() =>
            await _playlistService.GetAllAudiotracksFromPlaylist(new());

        // Assert
        await Assert.ThrowsAsync<PlaylistNotFoundException>(Action);
    }

    [Fact]
    public async Task GetPlaylistById_PlaylistExists_ReturnsPlaylist()
    {
        // Arrange
        var expectedPlaylist = PlaylistFabric.Create(
            MakeGuid(1),
            "Title",
            MakeGuid(1)
        );

        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(expectedPlaylist);

        // Act
        var actual = new Playlist(
            await _playlistService.GetPlaylistById(expectedPlaylist.Id)
        );

        // Assert
        Assert.Equal(expectedPlaylist, actual);
    }

    [Fact]
    public async Task GetPlaylistById_NoPlaylistWitId_Error()
    {
        // Arrange
        _mockPlaylistRepository
            .Setup(s => s.GetPlaylistById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Playlist)!);

        // Act
        async Task Action() => await _playlistService.GetPlaylistById(new());

        // Assert
        await Assert.ThrowsAsync<PlaylistNotFoundException>(Action);
    }

    [Fact]
    public async Task GetUserPlaylists_PlaylistsExist_ReturnsPlaylists()
    {
        // Arrange
        List<Playlist> expectedPlaylists =
        [
            PlaylistFabric.Create(MakeGuid(1), "Title", MakeGuid(1)),
        ];

        _mockUserRepository
            .Setup(s => s.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(UserFabric.CreateEmpty());
        _mockPlaylistRepository
            .Setup(s => s.GetUserPlaylists(It.IsAny<Guid>()))
            .ReturnsAsync(expectedPlaylists);

        // Act
        var actual = await _playlistService.GetUserPlaylists(new());

        // Assert
        Assert.Single(actual);
        Assert.Equal(expectedPlaylists, actual);
    }

    [Fact]
    public async Task GetUserPlaylists_NoUserWithId_Error()
    {
        // Arrange
        _mockUserRepository
            .Setup(s => s.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(default(User)!);

        // Act
        async Task Action() => await _playlistService.GetUserPlaylists(new());

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Action);
    }

    public static IEnumerable<object[]> GetUserPlaylistsContainingAudiotrack_Ok_GetTestData()
    {
        yield return new object[] { new List<Playlist>() };
        yield return new object[]
        {
            new List<Playlist>([PlaylistFabric.CreateEmpty()]),
        };
    }

    [Theory]
    [MemberData(nameof(GetUserPlaylistsContainingAudiotrack_Ok_GetTestData))]
    public async Task GetUserPlaylistsContainingAudiotrack_ReturnsFound(
        List<Playlist> expectedPlaylists
    )
    {
        // Arrange
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(AudiotrackFabric.CreateEmpty());
        _mockUserRepository
            .Setup(s => s.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(UserFabric.CreateEmpty());
        _mockPlaylistRepository
            .Setup(s => s.GetUserPlaylists(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Playlist>(expectedPlaylists));
        _mockPlaylistAudiotrackRepository
            .Setup(s =>
                s.IsAudiotrackInPlaylist(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(true);

        // Act
        var actual =
            await _playlistService.GetUserPlaylistsContainingAudiotrack(
                new(),
                new()
            );

        // Assert
        Assert.Equal(expectedPlaylists, actual);
    }

    public static IEnumerable<object[]> GetUserPlaylistsContainingAudiotrack_Error_GetTestData()
    {
        yield return new object[]
        {
            default(Audiotrack)!,
            UserFabric.CreateEmpty(),
            typeof(AudiotrackNotFoundException),
        };
        yield return new object[]
        {
            AudiotrackFabric.CreateEmpty(),
            default(User)!,
            typeof(UserNotFoundException),
        };
    }

    [Theory]
    [MemberData(nameof(GetUserPlaylistsContainingAudiotrack_Error_GetTestData))]
    public async Task GetUserPlaylistsContainingAudiotrack_NoUserWithId_Error(
        Audiotrack returnedAudiotrack,
        User returnedUser,
        Type expectedExceptionType
    )
    {
        // Arrange
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedAudiotrack);
        _mockUserRepository
            .Setup(s => s.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedUser);

        // Act
        var exception = await Assert.ThrowsAsync(
            expectedExceptionType,
            async () =>
            {
                await _playlistService.GetUserPlaylistsContainingAudiotrack(
                    new(),
                    new()
                );
            }
        );

        // Assert
        Assert.IsType(expectedExceptionType, exception);
    }
}
