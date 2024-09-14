using Microsoft.EntityFrameworkCore;

namespace MewingPad.Database.Context.Roles;

public class GuestDbContext(DbContextOptions<GuestDbContext> options) : MewingPadDbContext(options)
{
}