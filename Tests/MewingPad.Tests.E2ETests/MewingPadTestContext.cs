using MewingPad.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace MewingPad.Tests.E2ETests;

public class MewingPadTestContext : MewingPadDbContext
{
    public MewingPadTestContext(DbContextOptions<MewingPadDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
