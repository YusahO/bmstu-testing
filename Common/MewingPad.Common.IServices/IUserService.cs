using MewingPad.Common.Entities;
using MewingPad.Common.Enums;
namespace MewingPad.Services.UserService;

public interface IUserService
{
    Task<User> GetUserById(Guid userId);
    Task<User> GetUserByEmail(string userEmail);
    Task<User> ChangeUserPermissions(Guid userId, UserRole role = UserRole.Admin);
}