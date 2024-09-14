using MewingPad.Services.OAuthService;
using MewingPad.Common.Entities;
using MewingPad.DTOs.Converters;
using MewingPad.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using MewingPad.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Serilog;

namespace MewingPad.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService,
                            IOAuthService oauthService,
                            IConfiguration configuration) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IOAuthService _oauthService = oauthService;
    private readonly IConfiguration _config = configuration;
    private readonly Serilog.ILogger _logger = Log.ForContext<AuthController>();

    private string GenerateAccessToken(string userId, string userEmail, string userName)
    {
        var lifetime = _config.GetValue<double>("Jwt:AccessTokenValidityInSeconds");
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:Secret"]!));

        var expires = DateTime.UtcNow.Add(TimeSpan.FromHours(lifetime));

        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Email, userEmail),
                new(ClaimTypes.Name, userName)
            };

        var jwt = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    [AllowAnonymous]
    [HttpPost(nameof(Registration))]
    public async Task<IActionResult> Registration([FromBody] RegisterDto registerDto)
    {
        try
        {
            _logger.Information("Received {@Data}", new { registerDto.Username, registerDto.Email, registerDto.Role });
            var user = new User(Guid.NewGuid(),
                                registerDto.Username!,
                                registerDto.Email!,
                                registerDto.Password!,
                                registerDto.Role);

            user = await _oauthService.RegisterUser(user);
            var userAuthDto = new UserAuthDto
            {
                UserDto = UserConverter.CoreModelToDto(user),
                Token = GenerateAccessToken(user.Id.ToString(), user.Email, user.Username)
            };
            return Ok(userAuthDto);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost(nameof(Login))]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            _logger.Information("Received {@Data}", new { loginDto.Email });
            var user = await _oauthService.SignInUser(loginDto.Email!, loginDto.Password!);
            var userAuthDto = new UserAuthDto
            {
                UserDto = UserConverter.CoreModelToDto(user),
                Token = GenerateAccessToken(user.Id.ToString(), user.Email, user.Username)
            };
            return Ok(userAuthDto);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}