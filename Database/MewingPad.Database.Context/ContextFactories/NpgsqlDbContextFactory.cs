using MewingPad.Common.Enums;
using MewingPad.Database.Context.Roles;
using Microsoft.Extensions.Configuration;

namespace MewingPad.Database.Context;

public class NpgsqlDbContextFactory(GuestDbContext guestDbContext,
                                    UserDbContext userDbContext,
                                    AdminDbContext adminDbContext,
                                    IConfiguration config) : IDbContextFactory
{
    private readonly GuestDbContext _guestContext = guestDbContext;
    private readonly UserDbContext _userContext = userDbContext;
    private readonly AdminDbContext _adminContext = adminDbContext;
    private readonly IConfiguration _config = config;

    public MewingPadDbContext GetDbContext()
    {
        return _adminContext;
    }

    public MewingPadDbContext GetDbContext(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => _adminContext,
            UserRole.User => _userContext,
            _ => _guestContext,
        };
    }
}