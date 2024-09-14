using MewingPad.Common.Entities;

namespace MewingPad.Common.IRepositories;

public interface IReportRepository
{
    Task AddReport(Report report);
    Task<Report> UpdateReport(Report report);
    Task<Report?> GetReportById(Guid reportId);
    Task<List<Report>> GetAllReports();
}