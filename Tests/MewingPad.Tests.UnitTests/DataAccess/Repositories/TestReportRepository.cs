using MewingPad.Common.Enums;
using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using Moq.EntityFrameworkCore;

namespace MewingPad.Tests.UnitTests.DataAccess.Repositories;

public class TestReportRepository : BaseRepositoryTestClass
{
    private readonly ReportRepository _repository;
    private readonly MockDbContextFactory _mockFactory;

    public TestReportRepository()
    {
        _mockFactory = new MockDbContextFactory();
        _repository = new(_mockFactory.MockContext.Object);
    }

    private static Report CreateReportCoreModel(
        Guid id,
        Guid authorId,
        Guid audiotrackId,
        string text,
        ReportStatus status = ReportStatus.NotViewed
    )
    {
        return new ReportCoreModelBuilder()
            .WithId(id)
            .WithAuthorId(authorId)
            .WithAudiotrackId(audiotrackId)
            .WithText(text)
            .WithStatus(status)
            .Build();
    }

    private static ReportDbModel CreateReportDbo(
        Guid id,
        Guid authorId,
        Guid audiotrackId,
        string text,
        ReportStatus status = ReportStatus.NotViewed
    )
    {
        return new ReportDbModelBuilder()
            .WithId(id)
            .WithAuthorId(authorId)
            .WithAudiotrackId(audiotrackId)
            .WithText(text)
            .WithStatus(status)
            .Build();
    }

    private static ReportDbModel CreateReportDboFromCore(Report report)
    {
        return CreateReportDbo(
            report.Id,
            report.AuthorId,
            report.AudiotrackId,
            report.Text,
            report.Status
        );
    }

    [Fact]
    public async void AddReport_AddUnique_Ok()
    {
        // Arrange
        List<ReportDbModel> actual = [];
        var report = CreateReportCoreModel(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text"
        );
        var reportDbo = CreateReportDboFromCore(report);

        _mockFactory
            .MockReportsDbSet.Setup(s =>
                s.AddAsync(It.IsAny<ReportDbModel>(), default)
            )
            .Callback<ReportDbModel, CancellationToken>(
                (u, token) => actual.Add(u)
            );

        // Act
        await _repository.AddReport(report);

        // Assert
        Assert.Single(actual);
        Assert.Equal(report.Id, actual[0].Id);
        Assert.Equal(report.Text, actual[0].Text);
        Assert.Equal(report.Status, actual[0].Status);
        Assert.Equal(report.AuthorId, actual[0].AuthorId);
        Assert.Equal(report.AudiotrackId, actual[0].AudiotrackId);
    }

    [Fact]
    public async void AddReport_AddReportWithSameId_Error()
    {
        // Arrange
        _mockFactory
            .MockReportsDbSet.Setup(s =>
                s.AddAsync(It.IsAny<ReportDbModel>(), default)
            )
            .Callback<ReportDbModel, CancellationToken>(
                (r, token) => throw new RepositoryException()
            );

        // Act
        async Task Action() => await _repository.AddReport(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdateReport_UpdateExisting_Ok()
    {
        // Arrange
        var expectedId = MakeGuid(1);
        Guid expectedAuthorId = MakeGuid(1);
        Guid expectedAudiotrackId = MakeGuid(1);
        const string expectedText = "text";
        const ReportStatus expectedStatus = ReportStatus.Declined;

        var report = CreateReportCoreModel(
            expectedId,
            expectedAuthorId,
            expectedAudiotrackId,
            expectedText,
            ReportStatus.NotViewed
        );
        var reportDbo = CreateReportDboFromCore(report);
        List<ReportDbModel> reportDbos = [reportDbo];

        _mockFactory
            .MockReportsDbSet.Setup(s => s.Update(It.IsAny<ReportDbModel>()))
            .Callback(
                (ReportDbModel c) => reportDbos[0].Status = expectedStatus
            );

        // Act
        await _repository.UpdateReport(report);

        // Assert
        Assert.Single(reportDbos);
        Assert.Equal(expectedId, reportDbos[0].Id);
        Assert.Equal(expectedText, reportDbos[0].Text);
        Assert.Equal(expectedStatus, reportDbos[0].Status);
        Assert.Equal(expectedAuthorId, reportDbos[0].AuthorId);
        Assert.Equal(expectedAudiotrackId, reportDbos[0].AudiotrackId);
    }

    [Fact]
    public async void UpdateReport_UpdateNonexistent_Error()
    {
        // Arrange
        _mockFactory
            .MockReportsDbSet.Setup(s => s.Update(It.IsAny<ReportDbModel>()))
            .Callback((ReportDbModel c) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.UpdateReport(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    public static IEnumerable<object[]> GetReportById_GetTestData()
    {
        yield return new object[] { new ReportDbModel(), new Report() };
        yield return new object[]
        {
            CreateReportDbo(MakeGuid(1), MakeGuid(1), MakeGuid(1), "text"),
            CreateReportCoreModel(
                MakeGuid(1),
                MakeGuid(1),
                MakeGuid(1),
                "text"
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetReportById_GetTestData))]
    public async Task GetReportById_ReturnsFound(
        ReportDbModel returnedReportDbo,
        Report expectedReport
    )
    {
        // Arrange
        _mockFactory
            .MockReportsDbSet.Setup(x => x.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(returnedReportDbo);

        // Act
        var actual = await _repository.GetReportById(expectedReport.Id);

        // Assert
        Assert.Equal(expectedReport, actual);
    }

    public static IEnumerable<object[]> GetAllReports_GetTestData()
    {
        yield return new object[]
        {
            new List<ReportDbModel>(),
            new List<Report>(),
        };
        yield return new object[]
        {
            new List<ReportDbModel>(
                [CreateReportDbo(MakeGuid(1), MakeGuid(1), MakeGuid(1), "text")]
            ),
            new List<Report>(
                [
                    CreateReportCoreModel(
                        MakeGuid(1),
                        MakeGuid(1),
                        MakeGuid(1),
                        "text"
                    ),
                ]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetAllReports_GetTestData))]
    public async Task GetAllReports_ReturnsFound(
        List<ReportDbModel> reportDbos,
        List<Report> expectedReports
    )
    {
        // Arrange
        _mockFactory
            .MockContext.Setup(x => x.Reports)
            .ReturnsDbSet(reportDbos);

        // Act
        var actual = await _repository.GetAllReports();

        // Assert
        Assert.Equal(expectedReports, actual);
    }
}
