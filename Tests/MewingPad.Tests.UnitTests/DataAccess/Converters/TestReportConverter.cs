using MewingPad.Database.Models.Converters;


namespace MewingPad.Tests.UnitTests.DataAccess.Converters;

public class TestReportConverter
{
    [Fact]
    public void TestCoreToDbModel_Ok()
    {
        // Arrange
        var report = new ReportCoreModelBuilder()
            .WithId(new Guid())
            .WithText("report text")
            .WithAuthorId(new Guid())
            .WithAudiotrackId(new Guid())
            .WithStatus(Common.Enums.ReportStatus.NotViewed)
            .Build();

        // Act
        var actualResult = ReportConverter.CoreToDbModel(report);

        // Assert
        Assert.Equal(report.Id, actualResult.Id);
        Assert.Equal(report.Text, actualResult.Text);
        Assert.Equal(report.Status, actualResult.Status);
        Assert.Equal(report.AuthorId, actualResult.AuthorId);
        Assert.Equal(report.AudiotrackId, actualResult.AudiotrackId);
    }

    [Fact]
    public void TestDbToDbModel_Ok()
    {
        // Arrange
        var reportDbo = new ReportDbModelBuilder()
            .WithId(new Guid())
            .WithText("report text")
            .WithAuthorId(new Guid())
            .WithAudiotrackId(new Guid())
            .WithStatus(Common.Enums.ReportStatus.NotViewed)
            .Build();

        // Act
        var actualResult = ReportConverter.DbToCoreModel(reportDbo);

        // Assert
        Assert.Equal(reportDbo.Id, actualResult.Id);
        Assert.Equal(reportDbo.Text, actualResult.Text);
        Assert.Equal(reportDbo.Status, actualResult.Status);
        Assert.Equal(reportDbo.AuthorId, actualResult.AuthorId);
        Assert.Equal(reportDbo.AudiotrackId, actualResult.AudiotrackId);
    }

    [Fact]
    public void TestCoreToDbModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = ReportConverter.CoreToDbModel(null);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void TestDbToCoreModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = ReportConverter.DbToCoreModel(null);

        // Assert
        Assert.Null(actualResult);
    }
}