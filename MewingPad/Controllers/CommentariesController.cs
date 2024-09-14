using MewingPad.Services.CommentaryService;
using MewingPad.DTOs;
using MewingPad.DTOs.Converters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MewingPad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentariesController(ICommentaryService commentaryService) : ControllerBase
{
    private readonly ICommentaryService _commentaryService = commentaryService;
    private readonly Serilog.ILogger _logger = Log.ForContext<CommentariesController>();

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddCommentary([FromBody] CommentaryDto commentaryDto)
    {
        try
        {
            _logger.Information("Received {@Comm}", commentaryDto);
            var comm = CommentaryConverter.DtoToCoreModel(commentaryDto);
            comm.Id = Guid.NewGuid();
            await _commentaryService.CreateCommentary(comm);
            
            _logger.Information("Add commentary {@Comm}", comm);
            return Ok("Commentary added successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateCommentary([FromBody] CommentaryDto commentaryDto)
    {
        try
        {
            _logger.Information("Received {@Comm}", commentaryDto);
            var comm = CommentaryConverter.DtoToCoreModel(commentaryDto);
            await _commentaryService.UpdateCommentary(comm);

            _logger.Information("Updated commentary {@Comm}", comm);
            return Ok("Commentary updated successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{commentaryId:guid}")]
    public async Task<IActionResult> DeleteCommentary(Guid commentaryId)
    {
        try
        {
            _logger.Information("Received {@CommId}", commentaryId);
            await _commentaryService.DeleteCommentary(commentaryId);
            return Ok("Commentary deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}