namespace MewingPad.Database.Context;

public class TestDbContextFactory(MewingPadDbContext context) : IDbContextFactory
{
    private readonly MewingPadDbContext _context = context;

    public MewingPadDbContext GetDbContext()
    {
        return _context;
    }
}