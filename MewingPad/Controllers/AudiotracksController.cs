using MewingPad.Services.AudiotrackService;
using MewingPad.Services.CommentaryService;
using MewingPad.Services.PlaylistService;
using MewingPad.Services.ScoreService;
using MewingPad.Services.TagService;
using MewingPad.Services.UserService;
using MewingPad.DTOs;
using MewingPad.DTOs.Converters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MewingPad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudiotracksController(IAudiotrackService audiotrackService,
                                   ITagService tagService,
                                   IScoreService scoreService,
                                   IPlaylistService playlistService,
                                   ICommentaryService commentaryService,
                                   IUserService userService) : ControllerBase
{
    private readonly IAudiotrackService _audiotrackService = audiotrackService;
    private readonly ITagService _tagService = tagService;
    private readonly IScoreService _scoreService = scoreService;
    private readonly IPlaylistService _playlistService = playlistService;
    private readonly ICommentaryService _commentaryService = commentaryService;
    private readonly IUserService _userService = userService;

    private readonly Serilog.ILogger _logger = Log.ForContext<AudiotracksController>();

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllAudiotracks()
    {
        try
        {
            var audiotracks = from audiotrack in await _audiotrackService.GetAllAudiotracks()
                              select AudiotrackConverter.CoreModelToDto(audiotrack);
            return Ok(audiotracks);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("{audiotrackId:guid}")]
    public async Task<IActionResult> GetAudiotrack(Guid audiotrackId)
    {
        try
        {
            _logger.Information("Received {@AudiotrackId}", audiotrackId);
            var audiotrack = await _audiotrackService.GetAudiotrackById(audiotrackId);
            var audiotrackDto = AudiotrackConverter.CoreModelToDto(audiotrack);
            audiotrackDto.MeanScore = await _audiotrackService.GetAudiotrackMeanScore(audiotrackId);
            return Ok(audiotrackDto);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddAudiotrack([FromForm] AudiotrackFormDto audiotrackDto)
    {
        try
        {
            _logger.Information("Received {@Audio}", audiotrackDto);
            if (audiotrackDto.File is null || audiotrackDto.File.Length == 0)
            {
                _logger.Error("File is empty");
                throw new ArgumentException("File is empty");
            }
            using var memoryStream = new MemoryStream();
            await audiotrackDto.File.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var audiotrack = AudiotrackFormConverter.DtoToCoreModel(audiotrackDto);
            audiotrack.Id = Guid.NewGuid();
            await _audiotrackService.CreateAudiotrackWithStream(memoryStream, audiotrack);
            return Ok("Audiotrack uploaded successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{audiotrackId:guid}")]
    public async Task<IActionResult> DeleteAudiotrack(Guid audiotrackId)
    {
        try
        {
            _logger.Information("Received {@Audio}", audiotrackId);
            await _audiotrackService.DeleteAudiotrack(audiotrackId);
            return Ok("Audiotrack deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateAudiotrack([FromForm] AudiotrackFormDto audiotrackDto)
    {
        try
        {
            _logger.Information("Received {@Audio}", audiotrackDto);
            if (audiotrackDto.File is null || audiotrackDto.File.Length == 0)
            {
                _logger.Error("File is empty");
                throw new ArgumentException("File is empty");
            }
            using var memoryStream = new MemoryStream();
            await audiotrackDto.File.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var audiotrack = AudiotrackFormConverter.DtoToCoreModel(audiotrackDto);
            await _audiotrackService.UpdateAudiotrackWithStream(memoryStream, audiotrack);
            _logger.Information("Audiotrack {@Audio} updated successfully", audiotrack);

            return Ok("Audiotrack updated successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("{audiotrackId:guid}/tags")]
    public async Task<IActionResult> GetAudiotrackTags(Guid audiotrackId)
    {
        try
        {
            _logger.Information("Received {@Audio}", audiotrackId);
            var tags = from tag in await _tagService.GetAudiotrackTags(audiotrackId)
                       select TagConverter.CoreModelToDto(tag);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{audiotrackId:guid}/tags")]
    public async Task<IActionResult> AddTagToAudiotrack(Guid audiotrackId, [FromBody] Guid tagId)
    {
        try
        {
            _logger.Information("Received {@Data}", new { audiotrackId, tagId });
            await _tagService.AssignTagToAudiotrack(audiotrackId, tagId);
            return Ok("Tag added successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{audiotrackId:guid}/tags")]
    public async Task<IActionResult> DeleteTagFromAudiotrack(Guid audiotrackId, [FromBody] Guid tagId)
    {
        try
        {
            _logger.Information("Received {@Data}", new { audiotrackId, tagId });
            await _tagService.DeleteTagFromAudiotrack(audiotrackId, tagId);
            return Ok("Tag removed successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("{audiotrackId:guid}/scores")]
    public async Task<IActionResult> GetAllScores(Guid audiotrackId)
    {
        try
        {
            _logger.Information("Received {@AudioId}", audiotrackId);
            var scores = from score in await _scoreService.GetAudiotrackScores(audiotrackId)
                         select ScoreConverter.CoreModelToDto(score);
            return Ok(scores);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{audiotrackId:guid}/playlists")]
    public async Task<IActionResult> GetUserPlaylistsWithAudiotrack(Guid audiotrackId,
                                                                    [FromQuery] Guid userId)
    {
        try
        {
            _logger.Information("Received {@Data}", new { audiotrackId, userId });
            var playlists = await _playlistService.GetUserPlaylistsContainingAudiotrack(userId, audiotrackId);
            return Ok(playlists);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("{audiotrackId:guid}/commentaries")]
    public async Task<IActionResult> GetAudiotrackCommentaries(Guid audiotrackId)
    {
        try
        {
            _logger.Information("Received {@AudioId}", audiotrackId);
            var comms = (from comm in await _commentaryService.GetAudiotrackCommentaries(audiotrackId)
                         select CommentaryConverter.CoreModelToDto(comm)).ToList();

            for (int i = 0; i < comms.Count; ++i)
            {
                var username = (await _userService.GetUserById(comms[i].AuthorId)).Username;
                comms[i].AuthorName = username;
            }
            return Ok(comms);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}