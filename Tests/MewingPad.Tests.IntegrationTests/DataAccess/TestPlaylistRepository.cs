using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;


namespace MewingPad.Tests.IntegrationTests.DataAccess;

[Collection("Test Database")]
public class TestPlaylistRepository : BaseRepositoryTestClass
{
    private readonly PlaylistRepository _repository;

    public TestPlaylistRepository(DatabaseFixture fixture)
        : base(fixture)
    {
        _repository = new(Fixture.CreateContext());
    }

    [Fact]
    public async void AddPlaylist_AddToExisting_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(2);

        var playlist = new PlaylistCoreModelBuilder()
            .WithId(expectedId)
            .WithTitle("Hello")
            .WithUserId(DefaultUserId)
            .Build();

        // Act
        await _repository.AddPlaylist(playlist);

        // Assert
        var actual = (from a in context.Playlists select a).ToList();
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public async void AddPlaylist_AddPlaylistWithSameId_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(2);

        var playlist = new PlaylistCoreModelBuilder()
            .WithId(DefaultPlaylistId)
            .WithTitle("Hello")
            .WithUserId(DefaultUserId)
            .Build();

        // Act
        async Task Action() => await _repository.AddPlaylist(playlist);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void DeletePlaylist_DeleteExisting_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(2);

        var playlist = new PlaylistCoreModelBuilder()
            .WithId(expectedId)
            .WithTitle("Hello")
            .WithUserId(DefaultUserId)
            .Build();
        await context.Playlists.AddAsync(
            new PlaylistDbModelBuilder()
                .WithId(playlist.Id)
                .WithTitle(playlist.Title)
                .WithUserId(playlist.UserId)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        await _repository.DeletePlaylist(expectedId);

        // Assert
        Assert.Single((from a in context.Playlists select a).ToList());
    }

    [Fact]
    public async void DeletePlaylist_DeleteNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        async Task Action() => await _repository.DeletePlaylist(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdatePlaylist_UpdateExisting_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(2);
        const string expectedTitle = "New";

        var playlist = new PlaylistCoreModelBuilder()
            .WithId(expectedId)
            .WithTitle("Hello")
            .WithUserId(DefaultUserId)
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
            await context.SaveChangesAsync();
        }
        playlist.Title = expectedTitle;

        // Act
        await _repository.UpdatePlaylist(playlist);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            Assert.Equal(
                2,
                (from p in context.Playlists select p).ToList().Count
            );
            Assert.Single(
                (
                    from p in context.Playlists
                    where p.Title == expectedTitle
                    select p
                ).ToList()
            );
        }
    }

    [Fact]
    public async void UpdatePlaylist_UpdateNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(2);

        var playlist = new PlaylistCoreModelBuilder()
            .WithId(expectedId)
            .WithTitle("Hello")
            .WithUserId(expectedId)
            .Build();

        // Act
        async Task Action() => await _repository.UpdatePlaylist(playlist);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void GetAllPlaylists_OneExistingPlaylist_ReturnsPlaylist()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        // Act
        var actual = await _repository.GetAllPlaylists();

        // Assert
        Assert.Single(actual);
    }

    [Fact]
    public async void GetAllPlaylists_PlaylistsExist_ReturnsPlaylists()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        await context.Playlists.AddAsync(
            new PlaylistDbModelBuilder()
                .WithId(MakeGuid(2))
                .WithTitle($"Hello")
                .WithUserId(DefaultUserId)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAllPlaylists();

        // Assert
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public async void GetPlaylistById_PlaylistWithIdExists_ReturnsPlaylist()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = MakeGuid(2);
        var expectedUserId = DefaultUserId;
        const string expectedTitle = "Hello";

        await context.Playlists.AddAsync(
            new PlaylistDbModelBuilder()
                .WithId(expectedId)
                .WithTitle("Hello")
                .WithUserId(DefaultUserId)
                .Build()
        );

        context.SaveChanges();

        // Act
        var actual = await _repository.GetPlaylistById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal(expectedTitle, actual.Title);
        Assert.Equal(expectedUserId, actual.UserId);
    }

    [Fact]
    public async void GetPlaylistById_NoPlaylistsWithId_ReturnsNull()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = MakeGuid(2);

        // Act
        var actual = await _repository.GetPlaylistById(expectedId);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetPlaylistById_NoPlaylistsExist_ReturnsNull()
    {
        // Arrange

        // Act
        var actual = await _repository.GetPlaylistById(new Guid());

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetUserFavouritesPlaylist_PlaylistExists_ReturnsPlaylist()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = DefaultPlaylistId;
        var expectedUserId = DefaultUserId;
        const string expectedTitle = "Favourites";

        await context.Playlists.AddAsync(
            new PlaylistDbModelBuilder()
                .WithId(MakeGuid(2))
                .WithTitle("Hello")
                .WithUserId(DefaultUserId)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetUserFavouritesPlaylist(
            expectedUserId
        );

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal(expectedTitle, actual.Title);
        Assert.Equal(expectedUserId, actual.UserId);
    }

    [Fact]
    public async void GetUserPlaylists_NoCustomPlaylists_ReturnsFavourites()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = DefaultPlaylistId;
        var expectedUserId = DefaultUserId;
        const string expectedTitle = "Favourites";

        // Act
        var actual = await _repository.GetUserPlaylists(DefaultUserId);

        // Assert
        Assert.Single(actual);
        Assert.Equal(expectedId, actual[0].Id);
        Assert.Equal(expectedTitle, actual[0].Title);
        Assert.Equal(expectedUserId, actual[0].UserId);
    }

    [Fact]
    public async void GetUserPlaylists_CustomPlaylistsExist_ReturnsPlaylists()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = DefaultPlaylistId;
        var expectedUserId = DefaultUserId;

        await context.Playlists.AddAsync(
            new PlaylistDbModelBuilder()
                .WithId(MakeGuid(2))
                .WithTitle("Hello")
                .WithUserId(DefaultUserId)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetUserPlaylists(expectedUserId);

        // Assert
        Assert.Equal(2, actual.Count);
        Assert.All(
            actual,
            playlist => Assert.Equal(expectedUserId, playlist.UserId)
        );
    }
}
