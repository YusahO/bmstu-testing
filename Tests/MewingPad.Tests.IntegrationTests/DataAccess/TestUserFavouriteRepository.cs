using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;


namespace MewingPad.Tests.IntegrationTests.DataAccess;

[Collection("Test Database")]
public class TestUserFavouriteRepository : BaseRepositoryTestClass
{
    private UserFavouriteRepository _repository;

    public TestUserFavouriteRepository(DatabaseFixture fixture)
        : base(fixture)
    {
        _repository = new(Fixture.CreateContext());
    }

    [Fact]
    public async Task AddUserFavouritePlaylist_UserAndPlaylistExist_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedUserId = DefaultUserId;
        var expectedPlaylistId = DefaultPlaylistId;

        var userDbo = new UserDbModelBuilder()
            .WithId(expectedUserId)
            .WithUsername("username")
            .WithEmail("user@example.com")
            .WithPasswordHashed("passwd")
            .Build();
        var playlistDbo = new PlaylistDbModelBuilder()
            .WithId(expectedPlaylistId)
            .WithTitle("Title")
            .WithUserId(expectedUserId)
            .Build();

        await context.Users.AddAsync(userDbo);
        await context.Playlists.AddAsync(playlistDbo);
        await context.SaveChangesAsync();

        // Act
        await _repository.AddUserFavouritePlaylist(
            expectedUserId,
            expectedPlaylistId
        );

        // Assert
        var actual = (from uf in context.UsersFavourites select uf).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedUserId, actual[0].UserId);
        Assert.Equal(expectedPlaylistId, actual[0].FavouriteId);
    }

    [Fact]
    public async Task AddUserFavouritePlaylist_NoUser_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedUserId = DefaultUserId;
        var expectedPlaylistId = DefaultPlaylistId;

        var playlistDbo = new PlaylistDbModelBuilder()
            .WithId(expectedPlaylistId)
            .WithTitle("Title")
            .WithUserId(expectedUserId)
            .Build();

        await context.Playlists.AddAsync(playlistDbo);

        // Act
        async Task Action() =>
            await _repository.AddUserFavouritePlaylist(
                expectedUserId,
                expectedPlaylistId
            );

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task AddUserFavouritePlaylist_NoPlaylist_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedUserId = DefaultUserId;
        var expectedPlaylistId = DefaultPlaylistId;

        var userDbo = new UserDbModelBuilder()
            .WithId(expectedUserId)
            .WithUsername("username")
            .WithEmail("user@example.com")
            .WithPasswordHashed("passwd")
            .Build();

        await context.Users.AddAsync(userDbo);

        // Act
        async Task Action() =>
            await _repository.AddUserFavouritePlaylist(
                expectedUserId,
                expectedPlaylistId
            );

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task AddUserFavouritePlaylist_NoPlaylistNorUser_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedUserId = DefaultUserId;
        var expectedPlaylistId = DefaultPlaylistId;

        // Act
        async Task Action() =>
            await _repository.AddUserFavouritePlaylist(
                expectedUserId,
                expectedPlaylistId
            );

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }
}
