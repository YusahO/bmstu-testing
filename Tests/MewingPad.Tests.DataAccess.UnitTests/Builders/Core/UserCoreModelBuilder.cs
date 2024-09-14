namespace MewingPad.Tests.DataAccess.UnitTests.Builders;

using MewingPad.Common.Entities;
using MewingPad.Common.Enums;

public class UserCoreModelBuilder
{
    private User _user = new();

    public UserCoreModelBuilder WithId(Guid id)
    {
        _user.Id = id;
        return this;
    }

    public UserCoreModelBuilder WithUsername(string username)
    {
        _user.Username = username;
        return this;
    }

    public UserCoreModelBuilder WithPasswordHashed(string passwordHashed)
    {
        _user.PasswordHashed = passwordHashed;
        return this;
    }

    public UserCoreModelBuilder WithEmail(string email)
    {
        _user.Email = email;
        return this;
    }

    public UserCoreModelBuilder WithRole(UserRole role)
    {
        _user.Role = role;
        return this;
    }
    public User Build()
    {
        return _user;
    }
}

