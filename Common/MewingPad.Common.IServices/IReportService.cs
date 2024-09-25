using MewingPad.Common.Entities;
using MewingPad.Common.Enums;

namespace MewingPad.Services.ReportService;

public interface IReportService
{
    Task CreateReport(Report report);
    Task<Report> UpdateReportStatus(Guid reportId, ReportStatus status);
    Task<List<Report>> GetAllReports();
}