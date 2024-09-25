using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;

namespace MewingPad.Tests.UnitTests.DataAccess.Repositories;

public class TestUserFavouriteRepository : BaseRepositoryTestClass
{
    private UserFavouriteRepository _repository;
    private readonly MockDbContextFactory _mockFactory;

    public TestUserFavouriteRepository()
    {
        _mockFactory = new MockDbContextFactory();
        _repository = new(_mockFactory.MockContext.Object);
    }

    [Fact]
    public async Task AddUserFavouritePlaylist_AddUnique_Ok()
    {
        // Arrange
        List<UserFavouriteDbModel> userFavouriteDbos = [];
        var expectedUserId = MakeGuid(1);
        var expectedPlaylistId = MakeGuid(1);

        _mockFactory
            .MockUsersFavouritesDbSet.Setup(s =>
                s.AddAsync(It.IsAny<UserFavouriteDbModel>(), default)
            )
            .Callback<UserFavouriteDbModel, CancellationToken>(
                (uf, token) => userFavouriteDbos.Add(uf)
            );

        // Act
        await _repository.AddUserFavouritePlaylist(
            expectedUserId,
            expectedPlaylistId
        );

        // Assert
        Assert.Single(userFavouriteDbos);
        Assert.Equal(expectedUserId, userFavouriteDbos[0].UserId);
        Assert.Equal(expectedPlaylistId, userFavouriteDbos[0].FavouriteId);
    }

    [Fact]
    public async Task AddUserFavouritePlaylist_NoEntity_Error()
    {
        // Arrange
        _mockFactory
            .MockUsersFavouritesDbSet.Setup(s =>
                s.AddAsync(It.IsAny<UserFavouriteDbModel>(), default)
            )
            .Callback<UserFavouriteDbModel, CancellationToken>(
                (uf, token) => throw new RepositoryException()
            );

        // Act
        async Task Action() =>
            await _repository.AddUserFavouritePlaylist(new(), new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }
}
