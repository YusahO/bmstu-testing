using MewingPad.Services.ScoreService;
using MewingPad.DTOs;
using MewingPad.DTOs.Converters;
using MewingPad.Utils.Token;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MewingPad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoresController(IScoreService scoreService) : ControllerBase
{
    private readonly IScoreService _scoreService = scoreService
                                                   ?? throw new ArgumentNullException(nameof(scoreService));
    private readonly Serilog.ILogger _logger = Log.ForContext<ScoresController>();

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetScoreByPrimaryKey([FromQuery] Guid authorId,
                                                          [FromQuery] Guid audiotrackId)
    {
        try
        {
            _logger.Information("Received {@Data}", new { authorId, audiotrackId });
            var score = await _scoreService.GetScoreByPrimaryKey(authorId, audiotrackId);
            return Ok(ScoreConverter.CoreModelToDto(score));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateScore([FromBody] ScoreDto scoreDto)
    {
        try
        {
            _logger.Information("Received {@Score}", scoreDto);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            scoreDto.AuthorId = Guid.Parse(TokenUtils.TryGetClaimOfType(accessToken!));

            if ((await _scoreService.GetScoreByPrimaryKey(scoreDto.AuthorId, scoreDto.AudiotrackId)) is null)
            {
                await _scoreService.CreateScore(ScoreConverter.DtoToCoreModel(scoreDto));
                return Ok("Score created successfully");
            }
            else
            {
                await _scoreService.UpdateScore(ScoreConverter.DtoToCoreModel(scoreDto));
                return Ok("Score updated successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}