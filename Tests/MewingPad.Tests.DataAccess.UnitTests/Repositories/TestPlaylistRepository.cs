using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using Moq.EntityFrameworkCore;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

public class TestPlaylistRepository : BaseRepositoryTestClass
{
    private readonly PlaylistRepository _repository;
    private readonly MockDbContextFactory _mockFactory;

    public TestPlaylistRepository()
    {
        _mockFactory = new MockDbContextFactory();
        _repository = new(_mockFactory.MockContext.Object);
    }

    [Fact]
    public async void AddPlaylist_AddUnique_Ok()
    {
        // Arrange
        List<PlaylistDbModel> actual = [];
        var playlist = CreatePlaylistCoreModel(
            MakeGuid(1),
            "Title",
            MakeGuid(1)
        );
        var playlistDbo = CreatePlaylistDboFromCore(playlist);

        _mockFactory
            .MockPlaylistsDbSet.Setup(s =>
                s.AddAsync(It.IsAny<PlaylistDbModel>(), default)
            )
            .Callback<PlaylistDbModel, CancellationToken>(
                (p, token) => actual.Add(p)
            );

        // Act
        await _repository.AddPlaylist(playlist);

        // Assert
        Assert.Single(actual);
        Assert.Equal(playlist.Id, actual[0].Id);
        Assert.Equal(playlist.Title, actual[0].Title);
        Assert.Equal(playlist.UserId, actual[0].UserId);
    }

    [Fact]
    public async void AddPlaylist_AddPlaylistWithSameId_Error()
    {
        _mockFactory
            .MockPlaylistsDbSet.Setup(s =>
                s.AddAsync(It.IsAny<PlaylistDbModel>(), default)
            )
            .Callback<PlaylistDbModel, CancellationToken>(
                (a, token) => throw new RepositoryException()
            );

        // Act
        async Task Action() => await _repository.AddPlaylist(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void DeletePlaylist_DeleteExisting_Ok()
    {
        // Arrange
        Guid playlistId = MakeGuid(1);
        List<PlaylistDbModel> playlistDbos =
        [
            CreatePlaylistDbo(MakeGuid(1), "Title", MakeGuid(1)),
        ];

        _mockFactory
            .MockPlaylistsDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(playlistDbos[0]);
        _mockFactory
            .MockPlaylistsDbSet.Setup(s =>
                s.Remove(It.IsAny<PlaylistDbModel>())
            )
            .Callback((PlaylistDbModel a) => playlistDbos.Remove(a));

        // Act
        await _repository.DeletePlaylist(playlistId);

        // Assert
        Assert.Empty(playlistDbos);
    }

    [Fact]
    public async void DeletePlaylist_DeleteNonexistent_Error()
    {
        // Arrange
        _mockFactory
            .MockPlaylistsDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(PlaylistDbModel)!);
        _mockFactory
            .MockPlaylistsDbSet.Setup(s =>
                s.Remove(It.IsAny<PlaylistDbModel>())
            )
            .Callback((PlaylistDbModel a) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.DeletePlaylist(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdatePlaylist_UpdateExisting_Ok()
    {
        // Arrange
        var expectedId = MakeGuid(1);
        Guid expectedUserId = MakeGuid(1);
        const string expectedTitle = "New";

        var playlist = CreatePlaylistCoreModel(
            expectedId,
            "Old",
            expectedUserId
        );
        var playlistDbo = CreatePlaylistDboFromCore(playlist);
        List<PlaylistDbModel> playlistDbos = [playlistDbo];

        _mockFactory
            .MockPlaylistsDbSet.Setup(s =>
                s.Update(It.IsAny<PlaylistDbModel>())
            )
            .Callback(
                (PlaylistDbModel a) =>
                    playlistDbos[0].Title = new(expectedTitle)
            );

        // Act
        await _repository.UpdatePlaylist(playlist);

        // Assert
        Assert.Single(playlistDbos);
        Assert.Equal(expectedId, playlistDbos[0].Id);
        Assert.Equal(expectedTitle, playlistDbos[0].Title);
        Assert.Equal(expectedUserId, playlistDbos[0].UserId);
    }

    [Fact]
    public async void UpdatePlaylist_UpdateNonexistent_Error()
    {
        // Arrange
        var playlist = CreatePlaylistCoreModel(
            MakeGuid(1),
            "Title",
            MakeGuid(1)
        );

        _mockFactory
            .MockPlaylistsDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(PlaylistDbModel)!);
        _mockFactory
            .MockPlaylistsDbSet.Setup(s =>
                s.Update(It.IsAny<PlaylistDbModel>())
            )
            .Callback((PlaylistDbModel a) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.UpdatePlaylist(playlist);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    public static IEnumerable<object[]> GetAllPlaylists_GetTestData()
    {
        yield return new object[]
        {
            new List<PlaylistDbModel>(),
            new List<Playlist>(),
        };
        yield return new object[]
        {
            new List<PlaylistDbModel>(
                [CreatePlaylistDbo(MakeGuid(1), "Title", MakeGuid(1))]
            ),
            new List<Playlist>(
                [CreatePlaylistCoreModel(MakeGuid(1), "Title", MakeGuid(1))]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetAllPlaylists_GetTestData))]
    public async void GetAllPlaylists_ReturnsFound(
        List<PlaylistDbModel> playlistDbos,
        List<Playlist> expectedPlaylists
    )
    {
        // Arrange
        _mockFactory
            .MockContext.Setup(x => x.Playlists)
            .ReturnsDbSet(playlistDbos);

        // Act
        var actual = await _repository.GetAllPlaylists();

        // Assert
        Assert.Equal(expectedPlaylists, actual);
    }

    public static IEnumerable<object[]> GetPlaylistById_GetTestData()
    {
        yield return new object[] { new PlaylistDbModel(), new Playlist() };
        yield return new object[]
        {
            CreatePlaylistDbo(MakeGuid(1), "Title", MakeGuid(1)),
            CreatePlaylistCoreModel(MakeGuid(1), "Title", MakeGuid(1)),
        };
    }

    [Theory]
    [MemberData(nameof(GetPlaylistById_GetTestData))]
    public async Task GetPlaylistById_ReturnsFound(
        PlaylistDbModel returnedPlaylistDbo,
        Playlist expectedPlaylist
    )
    {
        // Arrange
        _mockFactory
            .MockPlaylistsDbSet.Setup(x => x.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(returnedPlaylistDbo);

        // Act
        var actual = await _repository.GetPlaylistById(expectedPlaylist.Id);

        // Assert
        Assert.Equal(expectedPlaylist, actual);
    }

    public static IEnumerable<object[]> GetUserFavouritesPlaylist_GetTestData()
    {
        yield return new object[]
        {
            new List<UserFavouriteDbModel>(),
            new List<PlaylistDbModel>(),
            default(Playlist)!,
        };
        yield return new object[]
        {
            new List<UserFavouriteDbModel>([new(MakeGuid(1), MakeGuid(1))]),
            new List<PlaylistDbModel>(
                [CreatePlaylistDbo(MakeGuid(1), "Title", MakeGuid(1))]
            ),
            CreatePlaylistCoreModel(MakeGuid(1), "Title", MakeGuid(1)),
        };
        yield return new object[]
        {
            new List<UserFavouriteDbModel>([new(MakeGuid(1), MakeGuid(1))]),
            new List<PlaylistDbModel>(
                [CreatePlaylistDbo(MakeGuid(2), "Title", MakeGuid(2))]
            ),
            default(Playlist)!,
        };
    }

    [Theory]
    [MemberData(nameof(GetUserFavouritesPlaylist_GetTestData))]
    public async void GetUserFavouritesPlaylist_ReturnsFound(
        List<UserFavouriteDbModel> usersFavourites,
        List<PlaylistDbModel> playlists,
        Playlist expectedPlaylist
    )
    {
        // Arrange
        var expectedUserId = MakeGuid(1);
        _mockFactory
            .MockContext.Setup(m => m.UsersFavourites)
            .ReturnsDbSet(usersFavourites);
        _mockFactory
            .MockContext.Setup(m => m.Playlists)
            .ReturnsDbSet(playlists);

        // Act
        var actual = await _repository.GetUserFavouritesPlaylist(
            expectedUserId
        );

        // Assert
        Assert.Equal(expectedPlaylist, actual);
    }

    public static IEnumerable<object[]> GetUserPlaylists_GetTestData()
    {
        yield return new object[]
        {
            new List<PlaylistDbModel>(),
            new List<Playlist>(),
        };
        yield return new object[]
        {
            new List<PlaylistDbModel>(
                [CreatePlaylistDbo(MakeGuid(1), "Title", MakeGuid(1))]
            ),
            new List<Playlist>(
                [CreatePlaylistCoreModel(MakeGuid(1), "Title", MakeGuid(1))]
            ),
        };
        yield return new object[]
        {
            new List<PlaylistDbModel>(
                [CreatePlaylistDbo(MakeGuid(1), "Title", MakeGuid(2))]
            ),
            new List<Playlist>(),
        };
    }

    [Theory]
    [MemberData(nameof(GetUserPlaylists_GetTestData))]
    public async void GetUserPlaylists_ReturnsFound(
        List<PlaylistDbModel> playlists,
        List<Playlist> expectedPlaylists
    )
    {
        // Arrange
        var expectedUserId = MakeGuid(1);
        _mockFactory
            .MockContext.Setup(m => m.Playlists)
            .ReturnsDbSet(playlists);

        // Act
        var actual = await _repository.GetUserPlaylists(expectedUserId);

        // Assert
        Assert.Equal(expectedPlaylists, actual);
    }
}
