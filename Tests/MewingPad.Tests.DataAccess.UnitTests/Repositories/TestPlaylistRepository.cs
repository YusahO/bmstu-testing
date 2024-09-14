using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Tests.DataAccess.UnitTests.Builders;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

[Collection("Test Database")]
public class TestPlaylistRepository : IDisposable
{
    public DatabaseFixture Fixture { get; }
    private readonly PlaylistRepository _repository;

    public TestPlaylistRepository(DatabaseFixture fixture)
    {
        Fixture = fixture;
        _repository = new(Fixture.CreateContext());
    }

    public void Dispose() => Fixture.Cleanup();

    [Fact]
    public async void TestAddPlaylist_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var playlist = new PlaylistCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("Hello")
            .WithUserId(Fixture.DefaultUserId)
            .Build();

        // Act
        await _repository.AddPlaylist(playlist);

        // Assert
        var actual = (from a in context.Playlists select a).ToList();
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public async void TestAddPlaylist_SamePlaylistError()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var playlist = new PlaylistCoreModelBuilder()
            .WithId(Fixture.DefaultFavouriteId)
            .WithTitle("Hello")
            .WithUserId(Fixture.DefaultUserId)
            .Build();

        // Act
        async Task Action() => await _repository.AddPlaylist(playlist);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void TestDeletePlaylist_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid userId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 1]);
        Guid playlistId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 1]);
        var playlist = new PlaylistCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("Hello")
            .WithUserId(Fixture.DefaultUserId)
            .Build();
        await context.Playlists.AddAsync(
            new PlaylistDbModelBuilder()
                .WithId(playlist.Id)
                .WithTitle(playlist.Title)
                .WithUserId(playlist.UserId)
                .Build()
        );
        context.SaveChanges();

        // Act
        await _repository.DeletePlaylist(playlist.Id);

        // Assert
        Assert.Single((from a in context.Playlists select a).ToList());
    }

    [Fact]
    public async void TestDeletePlaylist_NonexistentError()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        async Task Action() => await _repository.DeletePlaylist(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void TestUpdatePlaylist_Ok()
    {
        // Arrange
        var playlist = new PlaylistCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("Hello")
            .WithUserId(Fixture.DefaultUserId)
            .Build();
        using (var context = Fixture.CreateContext())
        {
            context.Playlists.Add(
                new PlaylistDbModelBuilder()
                    .WithId(playlist.Id)
                    .WithTitle(playlist.Title)
                    .WithUserId(playlist.UserId)
                    .Build()
            );
            context.SaveChanges();
        }

        playlist.Title = "New";

        // Act
        await _repository.UpdatePlaylist(playlist);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            var actual = (from a in context.Playlists select a).ToList();
            Assert.Equal(2, actual.Count);
        }
    }

    [Fact]
    public async void TestUpdatePlaylist_NonexistentError()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var playlist = new PlaylistCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("Hello")
            .WithUserId(Guid.NewGuid())
            .Build();

        // Act
        async Task Action() => await _repository.UpdatePlaylist(playlist);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void TestGetAllPlaylists_SingleOk()
    {
        // Arrange

        // Act
        var actual = await _repository.GetAllPlaylists();

        // Assert
        Assert.Single(actual);
    }

    [Fact]
    public async void TestGetAllPlaylists_SomeOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        for (int i = 0; i < 3; ++i)
        {
            await context.Playlists.AddAsync(
                new PlaylistDbModelBuilder()
                    .WithId(Guid.NewGuid())
                    .WithTitle($"Hello{i}")
                    .WithUserId(Fixture.DefaultUserId)
                    .Build()
            );
        }
        context.SaveChanges();

        // Act
        var actual = await _repository.GetAllPlaylists();

        // Assert
        Assert.Equal(4, actual.Count);
    }

    [Fact]
    public async void TestGetPlaylistById_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 3]);
        for (byte i = 2; i < 5; ++i)
        {
            await context.Playlists.AddAsync(
                new PlaylistDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithTitle($"Hello{i}")
                    .WithUserId(Fixture.DefaultUserId)
                    .Build()
            );
        }
        context.SaveChanges();

        // Act
        var actual = await _repository.GetPlaylistById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal("Hello3", actual.Title);
        Assert.Equal(Fixture.DefaultUserId, actual.UserId);
    }

    [Fact]
    public async void TestGetPlaylistById_NoneFoundOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 6]);
        for (byte i = 2; i < 5; ++i)
        {
            await context.Playlists.AddAsync(
                new PlaylistDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithTitle($"Hello{i}")
                    .WithUserId(Fixture.DefaultUserId)
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetPlaylistById(expectedId);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void TestGetPlaylistById_EmptyOk()
    {
        // Arrange

        // Act
        var actual = await _repository.GetPlaylistById(Guid.NewGuid());

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void TestGetUserFavouritesPlaylist_Ok()
    {
        using var context = Fixture.CreateContext();
        var data = (from up in context.UsersFavourites select up).ToList();
        foreach (var item in data)
        {
            Console.WriteLine($"{item.UserId}");
        }

        // Arrange
        for (byte i = 2; i < 5; ++i)
        {
            await context.Playlists.AddAsync(
                new PlaylistDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithTitle($"Hello{i}")
                    .WithUserId(Fixture.DefaultUserId)
                    .Build()
            );
        }
        context.SaveChanges();

        // Act
        var actual = await _repository.GetUserFavouritesPlaylist(
            Fixture.DefaultUserId
        );

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(Fixture.DefaultFavouriteId, actual.Id);
        Assert.Equal("Favourites", actual.Title);
        Assert.Equal(Fixture.DefaultUserId, actual.UserId);
    }

    [Fact]
    public async void TestGetUserPlaylists_OnlyFavouritesOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        var actual = await _repository.GetUserPlaylists(Fixture.DefaultUserId);

        // Assert
        Assert.Single(actual);
        Assert.Equal(Fixture.DefaultFavouriteId, actual[0].Id);
        Assert.Equal("Favourites", actual[0].Title);
        Assert.Equal(Fixture.DefaultUserId, actual[0].UserId);
    }

    [Fact]
    public async void TestGetUserPlaylists_ManyOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await context.Playlists.AddAsync(
            new PlaylistDbModelBuilder()
                .WithId(Guid.NewGuid())
                .WithTitle("Hello")
                .WithUserId(Fixture.DefaultUserId)
                .Build()
        );
        context.SaveChanges();

        // Act
        var actual = await _repository.GetUserPlaylists(Fixture.DefaultUserId);

        // Assert
        Assert.Equal(2, actual.Count);

        Assert.All(
            actual,
            (playlist) => Assert.Equal(Fixture.DefaultUserId, playlist.UserId)
        );
    }
}
