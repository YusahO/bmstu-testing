using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Database.Context;
using MewingPad.Database.Models;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Services.PlaylistService;
using MewingPad.Tests.Factories.Core;

namespace MewingPad.Tests.IntegrationTests.BusinessLogic;

[Collection("Test Database")]
public class PlaylistServiceIntegrationTest : BaseServiceTestClass
{
    private readonly MewingPadDbContext _context;
    private readonly PlaylistService _service;
    private readonly PlaylistRepository _playlistRepository;
    private readonly AudiotrackRepository _audiotrackRepository;
    private readonly PlaylistAudiotrackRepository _playlistAudiotrackRepository;
    private readonly UserRepository _userRepository;

    private readonly Guid DefaultNewPlaylistId = MakeGuid(2);

    private async Task AddDefaultNewPlaylist()
    {
        await _context.Playlists.AddAsync(
            new PlaylistDbModelBuilder()
                .WithId(DefaultNewPlaylistId)
                .WithTitle("Title")
                .WithUserId(DefaultUserId)
                .Build()
        );
        await _context.SaveChangesAsync();
    }

    public PlaylistServiceIntegrationTest(DatabaseFixture fixture)
        : base(fixture)
    {
        _context = Fixture.CreateContext();

        _playlistRepository = new(_context);
        _audiotrackRepository = new(_context);
        _playlistAudiotrackRepository = new(_context);
        _userRepository = new(_context);

        _service = new(
            _playlistRepository,
            _audiotrackRepository,
            _userRepository,
            _playlistAudiotrackRepository
        );
    }

    [Fact]
    public async Task CreatePlaylist_PlaylistUnique_Ok()
    {
        using var context = Fixture.CreateContext();
        // Arrange
        await AddDefaultUserWithPlaylist();
        var expectedPlaylist = new PlaylistCoreModelBuilder()
            .WithId(DefaultNewPlaylistId)
            .WithUserId(DefaultUserId)
            .WithTitle("Title")
            .Build();

        // Act
        await _service.CreatePlaylist(expectedPlaylist);

        // Assert
        var actual = (from p in context.Playlists select p).ToList();
        Assert.Equal(2, actual.Count);
        Assert.Equal(expectedPlaylist.Id, actual[1].Id);
    }

    [Fact]
    public async Task CreatePlaylist_PlaylistExists_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        var newPlaylist = PlaylistFabric.Create(
            DefaultPlaylistId,
            "Title",
            DefaultUserId
        );

        // Act
        async Task Action() => await _service.CreatePlaylist(newPlaylist);

        // Assert
        await Assert.ThrowsAsync<PlaylistExistsException>(Action);
    }

    [Fact]
    public async Task UpdatePlaylistTitle_PlaylistExists_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultNewPlaylist();

        var expectedId = DefaultNewPlaylistId;
        const string expectedTitle = "AAAA";

        // Act
        var actual = await _service.UpdatePlaylistTitle(
            expectedId,
            expectedTitle
        );

        // Assert
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal(expectedTitle, actual.Title);
    }

    [Fact]
    public async Task UpdatePlaylistTitle_PlaylistNonexistent_Error()
    {
        // Arrange

        // Act
        async Task Action() =>
            await _service.UpdatePlaylistTitle(new Guid(), "");

        // Assert
        await Assert.ThrowsAsync<PlaylistNotFoundException>(Action);
    }

    [Fact]
    public async Task DeletePlaylist_PlaylistExists_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultNewPlaylist();

        // Act
        await _service.DeletePlaylist(DefaultNewPlaylistId);

        // Assert
        using var context = Fixture.CreateContext();
        var actual = (from p in context.Playlists select p).ToList();
        Assert.Single(actual);
    }

    [Fact]
    public async Task DeletePlaylist_PlaylistNonexistent_Error()
    {
        // Arrange

        // Act
        async Task Action() => await _service.DeletePlaylist(new());

        // Assert
        await Assert.ThrowsAsync<PlaylistNotFoundException>(Action);
    }

    [Fact]
    public async Task AddAudiotrackToPlaylist_AddUnique_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedPlaylistId = DefaultPlaylistId;
        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act
        await _service.AddAudiotrackToPlaylist(
            expectedPlaylistId,
            expectedAudiotrackId
        );

        // Assert
        using var context = Fixture.CreateContext();
        var actual = (
            from pa in context.PlaylistsAudiotracks
            select pa
        ).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedPlaylistId, actual[0].PlaylistId);
        Assert.Equal(expectedAudiotrackId, actual[0].AudiotrackId);
    }

    [Fact]
    public async Task AddAudiotrackToPlaylist_NoAudiotrack_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        // Act
        async Task Action() =>
            await _service.AddAudiotrackToPlaylist(DefaultPlaylistId, new());

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    [Fact]
    public async Task AddAudiotrackToPlaylist_NoPlaylist_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        // Act
        async Task Action() =>
            await _service.AddAudiotrackToPlaylist(new(), DefaultAudiotrackId);

        // Assert
        await Assert.ThrowsAsync<PlaylistNotFoundException>(Action);
    }

    [Fact]
    public async Task AddAudiotrackToPlaylist_AudiotrackInPlaylist_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        using (var context = Fixture.CreateContext())
        {
            await context.PlaylistsAudiotracks.AddAsync(
                new PlaylistAudiotrackDbModel(
                    DefaultPlaylistId,
                    DefaultAudiotrackId
                )
            );
            await context.SaveChangesAsync();
        }

        var expectedPlaylistId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);

        // Act
        async Task Action() =>
            await _service.AddAudiotrackToPlaylist(
                expectedPlaylistId,
                expectedAudiotrackId
            );

        // Assert
        await Assert.ThrowsAsync<AudiotrackExistsInPlaylistException>(Action);
    }

    [Fact]
    public async Task RemoveAudiotrackFromPlaylist_DeleteExisting_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        using (var context = Fixture.CreateContext())
        {
            await context.PlaylistsAudiotracks.AddAsync(
                new PlaylistAudiotrackDbModel(
                    DefaultPlaylistId,
                    DefaultAudiotrackId
                )
            );
            await context.SaveChangesAsync();
        }

        var expectedPlaylistId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);

        // Act
        await _service.RemoveAudiotrackFromPlaylist(
            expectedPlaylistId,
            expectedAudiotrackId
        );

        // Assert
        var actual = (
            from pa in _context.PlaylistsAudiotracks
            select pa
        ).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task RemoveAudiotrackFromPlaylist_NoAudiotrack_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        // Act
        async Task Action() =>
            await _service.RemoveAudiotrackFromPlaylist(
                DefaultPlaylistId,
                new()
            );

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    [Fact]
    public async Task RemoveAudiotrackFromPlaylist_NoPlaylist_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        // Act
        async Task Action() =>
            await _service.RemoveAudiotrackFromPlaylist(
                new(),
                DefaultAudiotrackId
            );

        // Assert
        await Assert.ThrowsAsync<PlaylistNotFoundException>(Action);
    }

    [Fact]
    public async Task RemoveAudiotrackFromPlaylist_NoAudiotrackInPlaylist_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        // Act
        async Task Action() =>
            await _service.RemoveAudiotrackFromPlaylist(
                DefaultPlaylistId,
                DefaultAudiotrackId
            );

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundInPlaylistException>(Action);
    }

    [Fact]
    public async Task GetUserFavouritesPlaylist_UserAndPlaylistExist_ReturnsPlaylist()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        // Act
        var actual = await _service.GetUserFavouritesPlaylist(DefaultUserId);

        // Assert
        Assert.Equal(DefaultPlaylistId, actual.Id);
    }

    [Fact]
    public async Task GetUserFavouritesPlaylist_NoUser_Error()
    {
        // Arrange

        // Act
        async Task Action() => await _service.GetUserFavouritesPlaylist(new());

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAllAudiotracksFromPlaylist_ReturnsFoundAudiotracks()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        using (var context = Fixture.CreateContext())
        {
            await context.PlaylistsAudiotracks.AddAsync(
                new PlaylistAudiotrackDbModel(
                    DefaultPlaylistId,
                    DefaultAudiotrackId
                )
            );
            await context.SaveChangesAsync();
        }

        // Act
        var actual = await _service.GetAllAudiotracksFromPlaylist(
            DefaultPlaylistId
        );

        // Assert
        Assert.Single(actual);
        Assert.Equal(DefaultAudiotrackId, actual[0].Id);
    }

    [Fact]
    public async Task GetAllAudiotracksFromPlaylist_NoPlaylistWitId_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        using (var context = Fixture.CreateContext())
        {
            await context.PlaylistsAudiotracks.AddAsync(
                new PlaylistAudiotrackDbModel(
                    DefaultPlaylistId,
                    DefaultAudiotrackId
                )
            );
            await context.SaveChangesAsync();
        }

        // Act
        async Task Action() =>
            await _service.GetAllAudiotracksFromPlaylist(new());

        // Assert
        await Assert.ThrowsAsync<PlaylistNotFoundException>(Action);
    }

    [Fact]
    public async Task GetPlaylistById_PlaylistExists_ReturnsPlaylist()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = DefaultPlaylistId;

        // Act
        var actual = new Playlist(await _service.GetPlaylistById(expectedId));

        // Assert
        Assert.Equal(expectedId, actual.Id);
    }

    [Fact]
    public async Task GetPlaylistById_NoPlaylistWitId_Error()
    {
        // Arrange

        // Act
        async Task Action() => await _service.GetPlaylistById(new());

        // Assert
        await Assert.ThrowsAsync<PlaylistNotFoundException>(Action);
    }

    [Fact]
    public async Task GetUserPlaylists_PlaylistsExist_ReturnsPlaylists()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        // Act
        var actual = await _service.GetUserPlaylists(DefaultUserId);

        // Assert
        Assert.Single(actual);
        Assert.Equal(DefaultUserId, actual[0].UserId);
    }

    [Fact]
    public async Task GetUserPlaylists_NoUserWithId_Error()
    {
        // Arrange

        // Act
        async Task Action() => await _service.GetUserPlaylists(new());

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Action);
    }

    [Fact]
    public async Task GetUserPlaylistsContainingAudiotrack_ReturnsFound()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultNewPlaylist();
        await AddDefaultAudiotrack();

        using (var context = Fixture.CreateContext())
        {
            await context.PlaylistsAudiotracks.AddAsync(
                new PlaylistAudiotrackDbModel(
                    DefaultPlaylistId,
                    DefaultAudiotrackId
                )
            );
            await context.SaveChangesAsync();
        }

        // Act
        var actual = await _service.GetUserPlaylistsContainingAudiotrack(
            DefaultUserId,
            DefaultAudiotrackId
        );

        // Assert
        Assert.Single(actual);
        Assert.Equal(DefaultUserId, actual[0].UserId);
    }

    [Fact]
    public async Task GetUserPlaylistsContainingAudiotrack_NoUserWithId_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        // Act
        async Task Action() =>
            await _service.GetUserPlaylistsContainingAudiotrack(new(), DefaultAudiotrackId);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Action);
    }
}
