using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using MewingPad.Database.Context;
using MewingPad.Database.Models.Converters;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MewingPad.Database.NpgsqlRepositories;

public class ReportRepository(MewingPadDbContext context) : IReportRepository
{
    private readonly MewingPadDbContext _context = context;

    private readonly ILogger _logger = Log.ForContext<ReportRepository>();

    public async Task AddReport(Report report)
    {
        _logger.Verbose("Entering AddReport");

        try
        {
            await _context.Reports.AddAsync(ReportConverter.CoreToDbModel(report));
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting AddReport");
    }

    public async Task<List<Report>> GetAllReports()
    {
        _logger.Verbose("Entering GetAllReports");

        List<Report> reports;
        try
        {
            reports = await _context.Reports
                    .Select(r => ReportConverter.DbToCoreModel(r))
                    .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting GetAllReports");
        return reports;
    }

    public async Task<Report?> GetReportById(Guid reportId)
    {
        _logger.Verbose("Entering GetReportById");

        Report? report;

        try
        {
            var reportDbModel = await _context.Reports.FindAsync(reportId);
            report = ReportConverter.DbToCoreModel(reportDbModel);
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting GetReportById");
        return report;
    }

    public async Task<Report> UpdateReport(Report report)
    {
        _logger.Verbose("Entering UpdateReport");

        try
        {
            var reportDbModel = await _context.Reports.FindAsync(report.Id);

            reportDbModel!.AuthorId = report.AuthorId;
            reportDbModel!.AudiotrackId = report.AudiotrackId;
            reportDbModel!.Text = report.Text;
            reportDbModel!.Status = report.Status;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting UpdateReport");
        return report;
    }
}