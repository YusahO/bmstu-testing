using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using MewingPad.Utils.PasswordHasher;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace MewingPad.Services.OAuthService;

public class OAuthService(
    IConfiguration config,
    IUserRepository userRepository,
    IPlaylistRepository playlistRepository,
    IUserFavouriteRepository userFavouriteRepository
) : IOAuthService
{
    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException();
    private readonly IPlaylistRepository _playlistRepository =
        playlistRepository ?? throw new ArgumentNullException();
    private readonly IUserFavouriteRepository _userFavouriteRepository =
        userFavouriteRepository ?? throw new ArgumentNullException();
    private readonly ILogger _logger = Log.ForContext<OAuthService>();
    private readonly IConfiguration _config = config;
    private readonly string _favouritesName = config[
        "ApiSettings:FavouritesDefaultName"
    ]!;

    public async Task<User> RegisterUser(User user)
    {
        _logger.Verbose("Entering RegisterUser");

        var foundUser = await _userRepository.GetUserByEmail(user.Email);
        if (foundUser is not null)
        {
            _logger.Error(
                $"User with email \"{user.Email}\" already exists, cannot register"
            );
            throw new UserRegisteredException(
                $"User with email \"{user.Email}\" already registered"
            );
        }
        user.PasswordHashed = PasswordHasher.HashPassword(user.PasswordHashed);

        await _userRepository.AddUser(user);
        _logger.Information($"User (Id = {user.Id}) added");

        var favouritesId = Guid.NewGuid();
        await _playlistRepository.AddPlaylist(
            new(favouritesId, _favouritesName, user.Id)
        );
        _logger.Information(
            $"User favourites playlist (Id = {favouritesId}) added"
        );

        await _userFavouriteRepository.AddUserFavouritePlaylist(
            user.Id,
            favouritesId
        );

        _logger.Verbose("Exiting RegisterUser");
        return user;
    }

    public async Task<User> SignInUser(string email, string password)
    {
        _logger.Verbose($"Entering SignInUser({email})");
        var user = await _userRepository.GetUserByEmail(email);
        if (user is null)
        {
            _logger.Error($"User with email \"{email}\" not found");
            throw new UserNotFoundException(
                $"User with email \"{email}\" not found"
            );
        }

        if (!PasswordHasher.VerifyPassword(password, user.PasswordHashed))
        {
            _logger.Error($"Incorrect password for user \"{email}\"");
            throw new UserCredentialsException(
                $"Incorrect password for user with login \"{email}\""
            );
        }

        _logger.Verbose("Exiting SignInUser method");
        return user;
    }
}
