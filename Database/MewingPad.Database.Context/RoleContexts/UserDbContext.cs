using Microsoft.EntityFrameworkCore;

namespace MewingPad.Database.Context.Roles;

public class UserDbContext(DbContextOptions<UserDbContext> options) : MewingPadDbContext(options)
{
}