using MewingPad.Common.Entities;
using MewingPad.Common.Enums;

namespace MewingPad.Tests.Factories.Core;

public class UserFabric
{
    public static User Create(
        Guid id,
        string username,
        string email,
        string password,
        UserRole role = UserRole.User
    )
    {
        return new User(id, username, email, password, role);
    }

    public static User Copy(User other)
    {
        return new User(other);
    }

    public static User CreateEmpty()
    {
        return new User();
    }
}
