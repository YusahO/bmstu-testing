using MewingPad.Common.Exceptions;
using MewingPad.Services.PlaylistService;
using MewingPad.Services.UserService;
using MewingPad.DTOs.Converters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MewingPad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService,
                             IPlaylistService playlistService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IPlaylistService _playlistService = playlistService;
    private readonly Serilog.ILogger _logger = Log.ForContext<UsersController>();

    [Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> Get(Guid userId)
    {
        try
        {
            _logger.Information("Received {@UserId}", userId);
            var user = await _userService.GetUserById(userId);
            var favouritesId = (await _playlistService.GetUserFavouritesPlaylist(userId)).Id;

            var userDto = UserConverter.CoreModelToDto(user);
            userDto.FavouritesId = favouritesId;

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{email}")]
    public async Task<IActionResult> UpliftPrivileges(string email)
    {
        try
        {
            _logger.Information("Received {@UserEmail}", email);

            var user = await _userService.GetUserByEmail(email);
            await _userService.ChangeUserPermissions(user.Id);
            return Ok("User privileges uplifted");
        }
        catch (UserNotFoundException ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }


    [Authorize]
    [HttpGet("{userId:guid}/playlists")]
    public async Task<IActionResult> GetUserPlaylists(Guid userId)
    {
        try
        {
            _logger.Information("Received {@UserId}", userId);
            var playlists = await _playlistService.GetUserPlaylists(userId);
            return Ok(playlists);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
