using MewingPad.Common.Enums;
using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Tests.DataAccess.UnitTests.Builders;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

[Collection("Test Database")]
public class TestReportRepository : BaseRepositoryTestClass
{
    private readonly ReportRepository _repository;

    public TestReportRepository(DatabaseFixture fixture)
        : base(fixture)
    {
        _repository = new(Fixture.CreateContext());
    }

    [Fact]
    public async void AddReport_AddSingle_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(1);
        var expectedAuthorId = DefaultUserId;
        var expectedAudiotrackId = DefaultAudiotrackId;
        const ReportStatus expectedStatus = ReportStatus.NotViewed;
        const string expectedText = "Report";

        var report = new ReportCoreModelBuilder()
            .WithId(expectedId)
            .WithAuthorId(expectedAuthorId)
            .WithAudiotrackId(expectedAudiotrackId)
            .WithText(expectedText)
            .WithStatus(expectedStatus)
            .Build();

        // Act
        await _repository.AddReport(report);

        // Assert
        var actual = (from a in context.Reports select a).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedId, actual[0].Id);
        Assert.Equal(expectedText, actual[0].Text);
        Assert.Equal(expectedStatus, actual[0].Status);
        Assert.Equal(expectedAuthorId, actual[0].AuthorId);
        Assert.Equal(expectedAudiotrackId, actual[0].AudiotrackId);
    }

    [Fact]
    public async void AddReport_AddReportWithSameId_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(1);
        var expectedAuthorId = DefaultUserId;
        var expectedAudiotrackId = DefaultAudiotrackId;
        const ReportStatus expectedStatus = ReportStatus.NotViewed;
        const string expectedText = "Report";

        var report = new ReportCoreModelBuilder()
            .WithId(expectedId)
            .WithAuthorId(expectedAuthorId)
            .WithAudiotrackId(expectedAudiotrackId)
            .WithText(expectedText)
            .WithStatus(expectedStatus)
            .Build();
        await context.Reports.AddAsync(
            new ReportDbModelBuilder()
                .WithId(report.Id)
                .WithStatus(report.Status)
                .WithAuthorId(report.AuthorId)
                .WithAudiotrackId(report.AudiotrackId)
                .WithText(report.Text)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        async Task Action() => await _repository.AddReport(report);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdateReport_UpdateExisting_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(1);
        var expectedAuthorId = DefaultUserId;
        var expectedAudiotrackId = DefaultAudiotrackId;
        const ReportStatus expectedStatus = ReportStatus.Declined;
        const string expectedText = "Report";

        var report = new ReportCoreModelBuilder()
            .WithId(expectedId)
            .WithAuthorId(expectedAuthorId)
            .WithAudiotrackId(expectedAudiotrackId)
            .WithText(expectedText)
            .WithStatus(expectedStatus)
            .Build();
        using (var context = Fixture.CreateContext())
        {
            await context.Reports.AddAsync(
                new ReportDbModelBuilder()
                    .WithId(report.Id)
                    .WithAuthorId(report.AuthorId)
                    .WithAudiotrackId(report.AudiotrackId)
                    .WithText(report.Text)
                    .Build()
            );
            await context.SaveChangesAsync();
        }

        report.Status = expectedStatus;

        // Act
        await _repository.UpdateReport(report);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            var actual = (from a in context.Reports select a).ToList();
            Assert.Single(actual);
            Assert.Equal(expectedId, actual[0].Id);
            Assert.Equal(expectedText, actual[0].Text);
            Assert.Equal(expectedStatus, actual[0].Status);
            Assert.Equal(expectedAuthorId, actual[0].AuthorId);
            Assert.Equal(expectedAudiotrackId, actual[0].AudiotrackId);
        }
    }

    [Fact]
    public async void UpdateReport_UpdateNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var report = new ReportCoreModelBuilder()
            .WithId(MakeGuid(1))
            .WithAuthorId(DefaultUserId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .WithText("Report")
            .WithStatus(ReportStatus.NotViewed)
            .Build();

        // Act
        async Task Action() => await _repository.UpdateReport(report);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void GetReportById_ReportsExist_ReturnsReport()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(3);
        var expectedAuthorId = DefaultUserId;
        var expectedAudiotrackId = DefaultAudiotrackId;
        const ReportStatus expectedStatus = ReportStatus.Viewed;
        const string expectedText = "Report3";

        for (byte i = 1; i < 4; ++i)
        {
            await context.Reports.AddAsync(
                new ReportDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithAuthorId(DefaultUserId)
                    .WithAudiotrackId(DefaultAudiotrackId)
                    .WithText($"Report{i}")
                    .WithStatus(ReportStatus.Viewed)
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetReportById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal(expectedText, actual.Text);
        Assert.Equal(expectedStatus, actual.Status);
        Assert.Equal(expectedAuthorId, actual.AuthorId);
        Assert.Equal(expectedAudiotrackId, actual.AudiotrackId);
    }

    [Fact]
    public async void GetReportById_NoReportWithId_ReturnsNull()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(5);

        for (byte i = 1; i < 4; ++i)
        {
            await context.Reports.AddAsync(
                new ReportDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithAuthorId(DefaultUserId)
                    .WithAudiotrackId(DefaultAudiotrackId)
                    .WithText($"Report{i}")
                    .WithStatus(ReportStatus.Viewed)
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetReportById(expectedId);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetReportById_NoReports_ReturnsNull()
    {
        // Arrange

        // Act
        var actual = await _repository.GetReportById(new Guid());

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetAllReports_ReportsExist_ReturnReports()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        for (byte i = 1; i < 4; ++i)
        {
            await context.Reports.AddAsync(
                new ReportDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithAuthorId(DefaultUserId)
                    .WithAudiotrackId(DefaultAudiotrackId)
                    .WithText($"Report{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAllReports();

        // Assert
        Assert.Equal(3, actual.Count);
    }

    [Fact]
    public async void GetAllReports_NoReports_ReturnEmpty()
    {
        // Arrange

        // Act
        var actual = await _repository.GetAllReports();

        // Assert
        Assert.Empty(actual);
    }
}
