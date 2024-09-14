using MewingPad.Services.AudiotrackService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MewingPad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudiofilesController(IAudiotrackService audiotrackService) : ControllerBase
{
    private readonly IAudiotrackService _audiotrackService = audiotrackService;
    private readonly Serilog.ILogger _logger = Log.ForContext<AudiofilesController>();

    [AllowAnonymous]
    [HttpGet("{filename}")]
    public async Task<IActionResult> GetAudiotrackFile(string filename)
    {
        try
        {
            _logger.Information("Received {@Filename}", filename);
            var audioStream = await _audiotrackService.GetAudiotrackFileStream(filename);
            return File(audioStream, "application/octet-stream");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}