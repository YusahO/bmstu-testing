namespace MewingPad.Tests.DataAccess.UnitTests.Builders;

using MewingPad.Common.Enums;
using MewingPad.Database.Models;

public class UserDbModelBuilder
{
    private UserDbModel _userDbo =
        new()
        {
            Username = "username",
            Role = UserRole.User,
            PasswordHashed = "passwd",
            Email = "user@example.com",
        };

    public UserDbModelBuilder WithId(Guid id)
    {
        _userDbo.Id = id;
        return this;
    }

    public UserDbModelBuilder WithUsername(string username)
    {
        _userDbo.Username = username;
        return this;
    }

    public UserDbModelBuilder WithPasswordHashed(string passwordHashed)
    {
        _userDbo.PasswordHashed = passwordHashed;
        return this;
    }

    public UserDbModelBuilder WithEmail(string email)
    {
        _userDbo.Email = email;
        return this;
    }

    public UserDbModelBuilder WithRole(UserRole role)
    {
        _userDbo.Role = role;
        return this;
    }

    public UserDbModel Build()
    {
        return _userDbo;
    }
}
