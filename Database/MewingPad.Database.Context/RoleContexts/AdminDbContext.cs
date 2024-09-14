using Microsoft.EntityFrameworkCore;

namespace MewingPad.Database.Context.Roles;

public class AdminDbContext(DbContextOptions<AdminDbContext> options) : MewingPadDbContext(options)
{
}