using MewingPad.Database.Models.Converters;


namespace MewingPad.Tests.UnitTests.DataAccess.Converters;

public class TestPlaylistConverter
{
    [Fact]
    public void TestCoreToDbModel_Ok()
    {
        // Arrange
        var playlist = new PlaylistCoreModelBuilder()
            .WithId(new Guid())
            .WithTitle("Hello")
            .WithUserId(new Guid())
            .Build();

        // Act
        var actualResult = PlaylistConverter.CoreToDbModel(playlist);

        // Assert
        Assert.Equal(playlist.Id, actualResult.Id);
        Assert.Equal(playlist.Title, actualResult.Title);
        Assert.Equal(playlist.UserId, actualResult.UserId);
    }

    [Fact]
    public void TestDbToDbModel_Ok()
    {
        // Arrange
        var playlistDbo = new PlaylistDbModelBuilder()
            .WithId(new Guid())
            .WithTitle("Hello")
            .WithUserId(new Guid())
            .Build();

        // Act
        var actualResult = PlaylistConverter.DbToCoreModel(playlistDbo);

        // Assert
        Assert.Equal(playlistDbo.Id, actualResult.Id);
        Assert.Equal(playlistDbo.Title, actualResult.Title);
        Assert.Equal(playlistDbo.UserId, actualResult.UserId);
    }

    [Fact]
    public void TestCoreToDbModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = PlaylistConverter.CoreToDbModel(null);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void TestDbToCoreModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = PlaylistConverter.DbToCoreModel(null);

        // Assert
        Assert.Null(actualResult);
    }
}
