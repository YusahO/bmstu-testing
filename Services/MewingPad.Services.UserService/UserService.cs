using MewingPad.Common.Entities;
using MewingPad.Common.Enums;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using Serilog;

namespace MewingPad.Services.UserService;

public class UserService(IUserRepository repository) : IUserService
{
    private readonly IUserRepository _userRepository = repository
                                                       ?? throw new ArgumentNullException();

    private readonly ILogger _logger = Log.ForContext<UserService>();

    public async Task<User> ChangeUserPermissions(Guid userId, UserRole role = UserRole.Admin)
    {
        _logger.Verbose($"Entering ChangeUserPermissions({userId}, {role})");

        var user = await _userRepository.GetUserById(userId);
        if (user is null)
        {
            _logger.Error($"User (Id = {userId}) not found");
            throw new UserNotFoundException(userId);
        }
        user.Role = role;
        await _userRepository.UpdateUser(user);
        _logger.Information($"User (Id = {user.Id}) is admin: {role}");

        _logger.Verbose("Exiting ChangeUserPermissions");
        return user;
    }

    public async Task<User> GetUserById(Guid userId)
    {
        _logger.Verbose($"Entering GetUserById({userId})");

        var user = await _userRepository.GetUserById(userId);
        if (user is null)
        {
            _logger.Error($"User (Id = {userId}) not found");
            throw new UserNotFoundException(userId);
        }

        _logger.Verbose("Exiting GetUserById");
        return user;
    }

    public async Task<User> GetUserByEmail(string userEmail)
    {
        _logger.Verbose($"Entering GetUserByEmail({userEmail})");

        var user = await _userRepository.GetUserByEmail(userEmail);
        if (user is null)
        {
            _logger.Error($"User (Email = {userEmail}) not found");
            throw new UserNotFoundException($"User (Email = {userEmail}) not found");
        }

        _logger.Verbose("Exiting GetUserByEmail");
        return user;
    }
}
