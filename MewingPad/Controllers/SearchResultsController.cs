using MewingPad.Services.AudiotrackService;
using MewingPad.Services.TagService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace MewingPad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchResultsController(IAudiotrackService audiotrackService,
                                         ITagService tagService) : ControllerBase
    {
        private readonly IAudiotrackService _audiotrackService = audiotrackService;
        private readonly ITagService _tagService = tagService;
        private readonly Serilog.ILogger _logger = Log.ForContext<SearchResultsController>();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SearchAudiotracksByTags([FromBody] List<Guid> tagsIds)
        {
            try
            {
                _logger.Information("Received {@Tags}");
                if (tagsIds.Count == 0)
                {
                    return Ok(new List<object>());
                }
                var audiotracks = await _tagService.GetAudiotracksWithTags(tagsIds);
                return Ok(audiotracks);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception thrown {Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("{title}")]
        public async Task<IActionResult> SearchAudiotracksByTitle(string title)
        {
            try
            {
                _logger.Information("Received {@Title}", title);
                if (title.IsNullOrEmpty())
                {
                    return Ok(new List<object>());
                }
                var audiotracks = await _audiotrackService.GetAudiotracksByTitle(title);
                return Ok(audiotracks);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception thrown {Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}