using MewingPad.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace MewingPad.Tests.IntegrationTests;

public class DatabaseFixture
{
    private const string ConnectionString =
        @"Host=172.19.0.2;Port=5432;Username=postgres;Password=postgres;Database=testdb;Include Error Detail=true";

    public MewingPadDbContext CreateContext() =>
        new MewingPadDbContext(
            new DbContextOptionsBuilder<MewingPadDbContext>()
                .UseNpgsql(ConnectionString)
                .EnableSensitiveDataLogging()
                .Options
        );

    public DatabaseFixture()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        Cleanup();
    }

    public void Cleanup()
    {
        using var context = CreateContext();

        context.Users.RemoveRange(context.Users);
        context.Playlists.RemoveRange(context.Playlists);
        context.UsersFavourites.RemoveRange(context.UsersFavourites);
        context.SaveChanges();
    }
}

[CollectionDefinition("Test Database", DisableParallelization = true)]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
