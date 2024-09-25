using MewingPad.Database.Models.Converters;


namespace MewingPad.Tests.UnitTests.DataAccess.Converters;

public class TestAudiotrackConverter
{
    [Fact]
    public void TestCoreToDbModel_Ok()
    {
        // Arrange
        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(new Guid())
            .WithTitle("Hello")
            .WithAuthorId(new Guid())
            .WithFilepath("/path/to/track")
            .Build();

        // Act
        var actualResult = AudiotrackConverter.CoreToDbModel(audiotrack);

        // Assert
        Assert.Equal(audiotrack.Id, actualResult.Id);
        Assert.Equal(audiotrack.Title, actualResult.Title);
        Assert.Equal(audiotrack.AuthorId, actualResult.AuthorId);
        Assert.Equal(audiotrack.Filepath, actualResult.Filepath);
    }

    [Fact]
    public void TestDbToDbModel_Ok()
    {
        // Arrange
        var audiotrackDbo = new AudiotrackDbModelBuilder()
            .WithId(new Guid())
            .WithTitle("Hello")
            .WithAuthorId(new Guid())
            .WithFilepath("/path/to/track")
            .Build();

        // Act
        var actualResult = AudiotrackConverter.DbToCoreModel(audiotrackDbo);

        // Assert
        Assert.Equal(audiotrackDbo.Id, actualResult.Id);
        Assert.Equal(audiotrackDbo.Title, actualResult.Title);
        Assert.Equal(audiotrackDbo.AuthorId, actualResult.AuthorId);
        Assert.Equal(audiotrackDbo.Filepath, actualResult.Filepath);
    }

    [Fact]
    public void TestCoreToDbModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = AudiotrackConverter.CoreToDbModel(null);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void TestDbToCoreModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = AudiotrackConverter.DbToCoreModel(null);

        // Assert
        Assert.Null(actualResult);
    }
}