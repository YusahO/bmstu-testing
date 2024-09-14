namespace MewingPad.Database.Context;

public interface IDbContextFactory
{
    MewingPadDbContext GetDbContext();
}