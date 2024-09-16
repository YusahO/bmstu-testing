using MewingPad.Tests.DataAccess.UnitTests.Builders;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

public class BaseRepositoryTestClass(DatabaseFixture fixture) : IAsyncLifetime
{
    public DatabaseFixture Fixture { get; } = fixture;

    public Guid DefaultUserId = MakeGuid(1);
    public Guid DefaultPlaylistId = MakeGuid(1);
    public Guid DefaultAudiotrackId = MakeGuid(1);

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        Fixture.Cleanup();
        return Task.CompletedTask;
    }

    protected static Guid MakeGuid(byte i) =>
        new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]);

    protected async Task AddDefaultAudiotrack()
    {
        await AddAudiotrackWithId(DefaultAudiotrackId);
    }

    protected async Task AddDefaultUserWithPlaylist()
    {
        await AddUserWithPlaylistWithIds(DefaultUserId, DefaultPlaylistId);
    }

    protected async Task AddAudiotrackWithId(Guid audiotrackId)
    {
        using var context = Fixture.CreateContext();

        var audiotrack = new AudiotrackDbModelBuilder()
            .WithId(audiotrackId)
            .WithAuthorId(DefaultUserId)
            .WithFilepath("/path/to/file")
            .WithTitle("Title")
            .Build();
        await context.Audiotracks.AddAsync(audiotrack);
        await context.SaveChangesAsync();
    }

    protected async Task AddUserWithPlaylistWithIds(
        Guid userId,
        Guid playlistId
    )
    {
        using var context = Fixture.CreateContext();

        var user = new UserDbModelBuilder().WithId(userId).Build();
        var favourite = new PlaylistDbModelBuilder()
            .WithId(playlistId)
            .WithTitle("Favourites")
            .WithUserId(user.Id)
            .Build();
        await context.Users.AddAsync(user);
        await context.Playlists.AddAsync(favourite);
        await context.UsersFavourites.AddAsync(new(user.Id, favourite.Id));
        await context.SaveChangesAsync();
    }
}
