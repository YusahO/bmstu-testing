using MewingPad.Common.Enums;

namespace MewingPad.Tests.IntegrationTests.BusinessLogic;

public class BaseServiceTestClass(DatabaseFixture fixture) : IAsyncLifetime
{
    public DatabaseFixture Fixture { get; } = fixture;

    public Guid DefaultUserId = MakeGuid(1);
    public Guid DefaultPlaylistId = MakeGuid(1);
    public Guid DefaultAudiotrackId = MakeGuid(1);

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task InitializeAsync()
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

    protected async Task AddDefaultUserWithPlaylist(
        UserRole role = UserRole.User
    )
    {
        await AddUserWithPlaylistWithIds(
            DefaultUserId,
            DefaultPlaylistId,
            role
        );
    }

    protected async Task AddAudiotrackWithId(Guid audiotrackId)
    {
        using var context = Fixture.CreateContext();

        var audiotrack = new AudiotrackDbModelBuilder()
            .WithId(audiotrackId)
            .WithAuthorId(DefaultUserId)
            .WithFilepath("file.mp3")
            .WithTitle("Title")
            .Build();
        await context.Audiotracks.AddAsync(audiotrack);
        await context.SaveChangesAsync();
    }

    protected async Task AddUserWithPlaylistWithIds(
        Guid userId,
        Guid playlistId,
        UserRole role = UserRole.User
    )
    {
        using var context = Fixture.CreateContext();

        var user = new UserDbModelBuilder()
            .WithId(userId)
            .WithRole(role)
            .WithPasswordHashed(
                "$2a$11$hNEJrsowk93rMboSuIkb5u3WpRn0M1flrw4oVnl/54xRkLE1Uxdaa"
            )
            .Build();
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
