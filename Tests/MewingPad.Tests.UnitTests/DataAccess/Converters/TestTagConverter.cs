using MewingPad.Database.Models.Converters;


namespace MewingPad.Tests.UnitTests.DataAccess.Converters;

public class TestTagConverter
{
    [Fact]
    public void TestCoreToDbModel_Ok()
    {
        // Arrange
        var tag = new TagCoreModelBuilder()
            .WithId(new Guid())
            .WithName("tag")
            .WithAuthorId(new Guid())
            .Build();

        // Act
        var actualResult = TagConverter.CoreToDbModel(tag);

        // Assert
        Assert.Equal(tag.Id, actualResult.Id);
        Assert.Equal(tag.Name, actualResult.Name);
        Assert.Equal(tag.AuthorId, actualResult.AuthorId);
    }

    [Fact]
    public void TestDbToDbModel_Ok()
    {
        // Arrange
        var tagDbo = new TagDbModelBuilder()
            .WithId(new Guid())
            .WithName("tag")
            .WithAuthorId(new Guid())
            .Build();

        // Act
        var actualResult = TagConverter.DbToCoreModel(tagDbo);

        // Assert
        Assert.Equal(tagDbo.Id, actualResult.Id);
        Assert.Equal(tagDbo.Name, actualResult.Name);
        Assert.Equal(tagDbo.AuthorId, actualResult.AuthorId);
    }

    [Fact]
    public void TestCoreToDbModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = TagConverter.CoreToDbModel(null);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void TestDbToCoreModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = TagConverter.DbToCoreModel(null);

        // Assert
        Assert.Null(actualResult);
    }
}