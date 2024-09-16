using MewingPad.Database.Context;
using MewingPad.Database.Models;
using MewingPad.Tests.DataAccess.UnitTests.Builders;
using Microsoft.EntityFrameworkCore;

namespace MewingPad.Tests.DataAccess.UnitTests;

public class DatabaseFixture
{
    private const string ConnectionString =
        @"User ID=postgres;Password=postgres;Server=localhost;Database=MewingPadDBTest;";

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
