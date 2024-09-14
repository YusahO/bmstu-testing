using MewingPad.Common.Entities;

namespace MewingPad.Services.OAuthService;

public interface IOAuthService
{
    Task<User> RegisterUser(User user);
    Task RegisterAdmin(string name, string password);
    Task<User> SignInUser(string email, string password);
}
