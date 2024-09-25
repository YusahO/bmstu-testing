using MewingPad.Database.Models.Converters;


namespace MewingPad.Tests.DataAccess.UnitTests.Converters;

public class TestScoreConverter
{
    [Fact]
    public void TestCoreToDbModel_Ok()
    {
        // Arrange
        var score = new ScoreCoreModelBuilder()
            .WithAuthorId(new Guid())
            .WithAudiotrackId(new Guid())
            .WithValue(3)
            .Build();

        // Act
        var actualResult = ScoreConverter.CoreToDbModel(score);

        // Assert
        Assert.Equal(score.Value, actualResult.Value);
        Assert.Equal(score.AuthorId, actualResult.AuthorId);
        Assert.Equal(score.AudiotrackId, actualResult.AudiotrackId);
    }

    [Fact]
    public void TestDbToDbModel_Ok()
    {
        // Arrange
        var scoreDbo = new ScoreDbModelBuilder()
            .WithAuthorId(new Guid())
            .WithAudiotrackId(new Guid())
            .WithValue(3)
            .Build();

        // Act
        var actualResult = ScoreConverter.DbToCoreModel(scoreDbo);

        // Assert
        Assert.Equal(scoreDbo.Value, actualResult.Value);
        Assert.Equal(scoreDbo.AuthorId, actualResult.AuthorId);
        Assert.Equal(scoreDbo.AudiotrackId, actualResult.AudiotrackId);
    }

    [Fact]
    public void TestCoreToDbModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = ScoreConverter.CoreToDbModel(null);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void TestDbToCoreModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = ScoreConverter.DbToCoreModel(null);

        // Assert
        Assert.Null(actualResult);
    }
}