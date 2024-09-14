using MewingPad.Common.Enums;
using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Tests.DataAccess.UnitTests.Builders;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

[Collection("Test Database")]
public class TestReportRepository : IDisposable
{
    public DatabaseFixture Fixture { get; }
    public Guid AudiotrackId { get; } = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 1]);
    private readonly ReportRepository _repository;

    private void AddAudiotrack()
    {
        using var context = Fixture.CreateContext();

        context.Audiotracks.RemoveRange(context.Audiotracks);
        context.SaveChanges();

        var audiotrack = new AudiotrackDbModelBuilder()
            .WithId(AudiotrackId)
            .WithTitle("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
            .WithFilepath("/path/to/file")
            .Build();
        context.Audiotracks.Add(audiotrack);
        context.SaveChanges();
    }

    public TestReportRepository(DatabaseFixture fixture)
    {
        Fixture = fixture;
        _repository = new(Fixture.CreateContext());
        AddAudiotrack();
    }

    public void Dispose()
    {
        Fixture.Cleanup();
        AddAudiotrack();
    }

    [Fact]
    public async void TestAddReport_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var report = new ReportCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithAuthorId(Fixture.DefaultUserId)
            .WithAudiotrackId(AudiotrackId)
            .WithText("Report")
            .Build();

        // Act
        await _repository.AddReport(report);

        // Assert
        var actual = (from a in context.Reports select a).ToList();
        Assert.Single(actual);
    }

    [Fact]
    public async void TestAddReport_SameReportError()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var report = new ReportCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithAuthorId(Fixture.DefaultUserId)
            .WithAudiotrackId(AudiotrackId)
            .WithText("Report")
            .Build();
        await context.Reports.AddAsync(
            new ReportDbModelBuilder()
                .WithId(report.Id)
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
    public async void TestUpdateReport_Ok()
    {
        // Arrange
        var report = new ReportCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithAuthorId(Fixture.DefaultUserId)
            .WithAudiotrackId(AudiotrackId)
            .WithText("Report")
            .Build();
        using (var context = Fixture.CreateContext())
        {
            context.Reports.Add(
                new ReportDbModelBuilder()
                    .WithId(report.Id)
                    .WithAuthorId(report.AuthorId)
                    .WithAudiotrackId(report.AudiotrackId)
                    .WithText(report.Text)
                    .Build()
            );
            context.SaveChanges();
        }

        report.Status = ReportStatus.Declined;

        // Act
        await _repository.UpdateReport(report);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            var actual = (from a in context.Reports select a).ToList();
            Assert.Single(actual);
            Assert.Equal(report.Id, actual[0].Id);
            Assert.Equal(ReportStatus.Declined, actual[0].Status);
            Assert.Equal(report.AuthorId, actual[0].AuthorId);
            Assert.Equal(report.AudiotrackId, actual[0].AudiotrackId);
        }
    }

    [Fact]
    public async void TestUpdateReport_NonexistentError()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var report = new ReportCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithAuthorId(Fixture.DefaultUserId)
            .WithAudiotrackId(AudiotrackId)
            .WithText("Report")
            .Build();

        // Act
        async Task Action() => await _repository.UpdateReport(report);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void TestGetReportById_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 2]);
        for (byte i = 1; i < 4; ++i)
        {
            await context.Reports.AddAsync(
                new ReportDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithAudiotrackId(Fixture.DefaultUserId)
                    .WithText($"Report{i}")
                    .Build()
            );
        }
        context.SaveChanges();

        // Act
        var actual = await _repository.GetReportById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal("Report2", actual.Text);
        Assert.Equal(AudiotrackId, actual.AudiotrackId);
        Assert.Equal(Fixture.DefaultUserId, actual.AuthorId);
    }

    [Fact]
    public async void TestGetReportById_NoneFoundOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 5]);
        for (byte i = 1; i < 4; ++i)
        {
            await context.Reports.AddAsync(
                new ReportDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithAudiotrackId(AudiotrackId)
                    .WithText($"Report{i}")
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
    public async void TestGetReportById_EmptyOk()
    {
        // Arrange

        // Act
        var actual = await _repository.GetReportById(Guid.NewGuid());

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void TestGetAllReports_SomeOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        for (byte i = 1; i < 4; ++i)
        {
            await context.Reports.AddAsync(
                new ReportDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithAudiotrackId(AudiotrackId)
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
    public async void TestGetAllReports_EmptyOk()
    {
        // Arrange

        // Act
        var actual = await _repository.GetAllReports();

        // Assert
        Assert.Empty(actual);
    }
}
