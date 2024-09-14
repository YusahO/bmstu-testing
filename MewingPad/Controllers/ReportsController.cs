using MewingPad.Services.ReportService;
using MewingPad.DTOs;
using MewingPad.DTOs.Converters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MewingPad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    private readonly IReportService _reportService = reportService;
    private readonly Serilog.ILogger _logger = Log.ForContext<ReportsController>();

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllReports()
    {
        try
        {
            var reports = await _reportService.GetAllReports();
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddReport([FromBody] ReportDto reportDto)
    {
        try
        {
            _logger.Information("Received {@Report}", reportDto);
            var report = ReportConverter.DtoToCoreModel(reportDto);
            report.Id = Guid.NewGuid();
            await _reportService.CreateReport(report);
            return Ok("Report added successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateReport([FromBody] ReportDto reportDto)
    {
        try
        {
            _logger.Information("Received {@Report}", reportDto);
            var report = ReportConverter.DtoToCoreModel(reportDto);
            await _reportService.UpdateReportStatus(report.Id, report.Status);
            return Ok("Report updated successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}