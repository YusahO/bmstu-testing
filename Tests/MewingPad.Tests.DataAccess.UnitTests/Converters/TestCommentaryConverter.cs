using MewingPad.Database.Models.Converters;
using MewingPad.Tests.DataAccess.UnitTests.Builders;

namespace MewingPad.Tests.DataAccess.UnitTests.Controllers;

public class TestCommentaryConverter
{
    [Fact]
    public void TestCoreToDbModel_Ok()
    {
        // Arrange
        var commentary = new CommentaryCoreModelBuilder()
            .WithId(new Guid())
            .WithText("text")
            .WithAuthorId(new Guid())
            .WithAudiotrackId(new Guid())
            .Build();

        // Act
        var actualResult = CommentaryConverter.CoreToDbModel(commentary);

        // Assert
        Assert.Equal(commentary.Id, actualResult.Id);
        Assert.Equal(commentary.Text, actualResult.Text);
        Assert.Equal(commentary.AuthorId, actualResult.AuthorId);
        Assert.Equal(commentary.AudiotrackId, actualResult.AudiotrackId);
    }

    [Fact]
    public void TestDbToDbModel_Ok()
    {
        // Arrange
        var commentaryDbo = new CommentaryDbModelBuilder()
            .WithId(new Guid())
            .WithText("text")
            .WithAuthorId(new Guid())
            .WithAudiotrackId(new Guid())
            .Build();

        // Act
        var actualResult = CommentaryConverter.DbToCoreModel(commentaryDbo);

        // Assert
        Assert.Equal(commentaryDbo.Id, actualResult.Id);
        Assert.Equal(commentaryDbo.Text, actualResult.Text);
        Assert.Equal(commentaryDbo.AuthorId, actualResult.AuthorId);
        Assert.Equal(commentaryDbo.AudiotrackId, actualResult.AudiotrackId);
    }

    [Fact]
    public void TestCoreToDbModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = CommentaryConverter.CoreToDbModel(null);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void TestDbToCoreModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = CommentaryConverter.DbToCoreModel(null);

        // Assert
        Assert.Null(actualResult);
    }
}