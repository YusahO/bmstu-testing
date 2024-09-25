using MewingPad.Common.Enums;
using MewingPad.Common.Exceptions;
using MewingPad.Services.ReportService;
using MewingPad.Tests.Factories.Core;

namespace MewingPad.Tests.UnitTests.BusinessLogic.Services;

internal class InMemoryReportRepository() : IReportRepository
{
    public List<Report> Reports = [];

    public Task AddReport(Report report)
    {
        Reports.Add(report);
        return Task.CompletedTask;
    }

    public Task<List<Report>> GetAllReports()
    {
        return Task.FromResult(Reports);
    }

    public Task<Report?> GetReportById(Guid reportId)
    {
        return Task.FromResult(Reports.FirstOrDefault(r => r.Id == reportId));
    }

    public Task<Report> UpdateReport(Report report)
    {
        int found = Reports.FindIndex(0, r => r.Id == report.Id);
        Reports[found] = report;
        return Task.FromResult(new Report(report));
    }
}

public class ReportServiceUnitTest : BaseServiceTestClass
{
    private readonly ReportService _reportService;
    private readonly InMemoryReportRepository _reportRepository;

    public ReportServiceUnitTest()
    {
        _reportRepository = new();
        _reportService = new(_reportRepository);
    }

    [Fact]
    public async Task CreateReport_ReportUnique_Ok()
    {
        // Arrange
        var expectedReport = ReportFabric.Create(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text",
            ReportStatus.NotViewed
        );

        // Act
        await _reportService.CreateReport(expectedReport);

        // Assert
        Assert.Single(_reportRepository.Reports);
    }

    [Fact]
    public async Task CreateReport_CreateExisting_Error()
    {
        // Arrange
        var expectedReport = ReportFabric.Create(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text",
            ReportStatus.NotViewed
        );
        _reportRepository.Reports = [expectedReport];

        // Act
        async Task Action() =>
            await _reportService.CreateReport(expectedReport);

        // Assert
        await Assert.ThrowsAsync<ReportExistsException>(Action);
    }

    [Fact]
    public async Task UpdateReportStatus_ReportExists_Ok()
    {
        // Arrange
        var expectedStatus = ReportStatus.Accepted;
        var oldReport = ReportFabric.Create(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text",
            ReportStatus.NotViewed
        );
        _reportRepository.Reports = [new(oldReport)];

        // Act
        await _reportService.UpdateReportStatus(oldReport.Id, expectedStatus);

        // Assert
        var reports = _reportRepository.Reports;
        Assert.Single(reports);
        Assert.Equal(expectedStatus, reports[0].Status);
        Assert.Equal(oldReport.Id, reports[0].Id);
        Assert.Equal(oldReport.AuthorId, reports[0].AuthorId);
        Assert.Equal(oldReport.AudiotrackId, reports[0].AudiotrackId);
        Assert.Equal(oldReport.Text, reports[0].Text);
    }

    [Fact]
    public async Task UpdateReportStatus_UpdateNonexistent_Error()
    {
        // Arrange

        // Act
        async Task Action() =>
            await _reportService.UpdateReportStatus(
                new(),
                ReportStatus.Accepted
            );

        // Assert
        await Assert.ThrowsAsync<ReportNotFoundException>(Action);
    }

    public static IEnumerable<object[]> GetAllReports_GetTestData()
    {
        yield return new object[]
        {
            new List<Report>(
                [
                    ReportFabric.Create(
                        MakeGuid(1),
                        MakeGuid(1),
                        MakeGuid(1),
                        "Text",
                        ReportStatus.NotViewed
                    ),
                ]
            ),
        };

        yield return new object[] { new List<Report>([]) };
    }

    [Theory]
    [MemberData(nameof(GetAllReports_GetTestData))]
    public async Task GetAllReports_ReturnsReports(List<Report> expectedReports)
    {
        // Arrange
        _reportRepository.Reports = new(expectedReports);

        // Act
        var actual = await _reportService.GetAllReports();

        // Assert
        Assert.Equal(expectedReports, actual);
    }
}
