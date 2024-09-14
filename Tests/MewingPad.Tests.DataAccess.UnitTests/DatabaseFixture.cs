using MewingPad.Database.Context;
using MewingPad.Database.Models;
using MewingPad.Tests.DataAccess.UnitTests.Builders;
using Microsoft.EntityFrameworkCore;

namespace MewingPad.Tests.DataAccess.UnitTests;

public class DatabaseFixture
{
    private const string ConnectionString =
        @"User ID=postgres;Password=postgres;Server=localhost;Database=MewingPadDBTest;";

    public Guid DefaultUserId { get; } = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 1]);
    public Guid DefaultFavouriteId { get; } =
        new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 1]);

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

        var user = new UserDbModelBuilder().WithId(DefaultUserId).Build();
        var favourite = new PlaylistDbModelBuilder()
            .WithId(DefaultFavouriteId)
            .WithTitle("Favourites")
            .WithUserId(user.Id)
            .Build();
        context.Users.Add(user);
        context.Playlists.Add(favourite);
        context.UsersFavourites.Add(
            new UserFavouriteDbModel(user.Id, favourite.Id)
        );
        context.SaveChanges();
    }
}

[CollectionDefinition("Test Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
