using MewingPad.Common.Enums;
using MewingPad.Common.Exceptions;
using MewingPad.Database.Context;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Services.ReportService;
using MewingPad.Tests.Factories.Core;

namespace MewingPad.Tests.IntegrationTests.BusinessLogic;

[Collection("Test Database")]
public class ReportServiceIntegrationTest : BaseServiceTestClass
{
    private readonly MewingPadDbContext _context;
    private readonly ReportService _service;
    private readonly ReportRepository _reportRepository;

    private readonly Guid DefaultReportId = MakeGuid(1);

    private async Task AddDefaultReport()
    {
        await _context.Reports.AddAsync(
            new ReportDbModelBuilder()
                .WithId(DefaultReportId)
                .WithAuthorId(DefaultUserId)
                .WithAudiotrackId(DefaultAudiotrackId)
                .WithText("Text")
                .WithStatus(ReportStatus.NotViewed)
                .Build()
        );
        await _context.SaveChangesAsync();
    }

    public ReportServiceIntegrationTest(DatabaseFixture fixture)
        : base(fixture)
    {
        _context = Fixture.CreateContext();

        _reportRepository = new(_context);

        _service = new(_reportRepository);
    }

    [Fact]
    public async Task CreateReport_ReportUnique_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var report = ReportFabric.Create(
            MakeGuid(1),
            DefaultUserId,
            DefaultAudiotrackId,
            "Text",
            ReportStatus.Declined
        );

        // Act
        await _service.CreateReport(report);

        // Assert
        var actual = (from r in _context.Reports select r).ToList();
        Assert.Single(actual);
        Assert.Equal(report.Id, actual[0].Id);
    }

    [Fact]
    public async Task CreateReport_CreateExisting_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultReport();

        var report = ReportFabric.Create(
            MakeGuid(1),
            DefaultUserId,
            DefaultAudiotrackId,
            "Text",
            ReportStatus.Declined
        );

        // Act
        async Task Action() => await _service.CreateReport(report);

        // Assert
        await Assert.ThrowsAsync<ReportExistsException>(Action);
    }

    [Fact]
    public async Task UpdateReportStatus_ReportExists_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultReport();

        var expectedId = DefaultReportId;
        var expectedStatus = ReportStatus.Accepted;

        // Act
        await _service.UpdateReportStatus(expectedId, expectedStatus);

        // Assert
        var actual = (from r in _context.Reports select r).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedId, actual[0].Id);
        Assert.Equal(expectedStatus, actual[0].Status);
    }

    [Fact]
    public async Task UpdateReportStatus_UpdateNonexistent_Error()
    {
        // Arrange

        // Act
        async Task Action() =>
            await _service.UpdateReportStatus(new(), ReportStatus.Accepted);

        // Assert
        await Assert.ThrowsAsync<ReportNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAllReports_ReportsExist_ReturnsReports()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultReport();

        // Act
        var actual = await _service.GetAllReports();

        // Assert
        Assert.Single(actual);
    }

    [Fact]
    public async Task GetAllReports_NoReports_ReturnsEmpty()
    {
        // Arrange

        // Act
        var actual = await _service.GetAllReports();

        // Assert
        Assert.Empty(actual);
    }
}
