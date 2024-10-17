using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;


namespace MewingPad.Tests.IntegrationTests.DataAccess;

[Collection("Test Database")]
public class TestPlaylistAudiotrackRepository : BaseRepositoryTestClass
{
    private PlaylistAudiotrackRepository _repository;

    public TestPlaylistAudiotrackRepository(DatabaseFixture fixture)
        : base(fixture)
    {
        _repository = new(Fixture.CreateContext());
    }

    private async Task AddPlaylistWithUserId(Guid playlistId, Guid userId)
    {
        using var context = Fixture.CreateContext();
        var playlistDbo = new PlaylistDbModelBuilder()
            .WithId(playlistId)
            .WithTitle($"Playlist_{playlistId}")
            .WithUserId(userId)
            .Build();
        await context.Playlists.AddAsync(playlistDbo);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddAudiotrackToPlaylist_AddUnique_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedPlaylistId = DefaultPlaylistId;
        var expectedAudiotrackId = MakeGuid(1);

        await AddDefaultUserWithPlaylist();
        await AddAudiotrackWithId(expectedAudiotrackId);

        // Act
        await _repository.AddAudiotrackToPlaylist(
            expectedAudiotrackId,
            expectedPlaylistId
        );

        // Assert
        var actual = (
            from pa in context.PlaylistsAudiotracks
            where pa.PlaylistId == expectedPlaylistId
            select pa
        ).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedPlaylistId, actual[0].PlaylistId);
        Assert.Equal(expectedAudiotrackId, actual[0].AudiotrackId);
    }

    [Fact]
    public async Task AddAudiotrackToPlaylist_AddExisting_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedPlaylistId = DefaultPlaylistId;
        var expectedAudiotrackId = MakeGuid(1);

        await AddDefaultUserWithPlaylist();
        await AddAudiotrackWithId(expectedAudiotrackId);

        await context.PlaylistsAudiotracks.AddAsync(
            new(expectedPlaylistId, expectedAudiotrackId)
        );
        await context.SaveChangesAsync();

        // Act
        async Task Action() =>
            await _repository.AddAudiotrackToPlaylist(
                expectedPlaylistId,
                expectedAudiotrackId
            );

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task RemoveAudiotrackFromPlaylist_AudiotrackAndPlaylistExist_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedPlaylistId = DefaultPlaylistId;
        var expectedAudiotrackId = MakeGuid(1);

        await AddDefaultUserWithPlaylist();
        await AddAudiotrackWithId(expectedAudiotrackId);
        await context.PlaylistsAudiotracks.AddAsync(
            new(expectedPlaylistId, expectedAudiotrackId)
        );
        await context.SaveChangesAsync();

        // Act
        await _repository.RemoveAudiotrackFromPlaylist(
            expectedPlaylistId,
            expectedAudiotrackId
        );

        // Assert
        var actual = (
            from pa in context.PlaylistsAudiotracks
            where pa.PlaylistId == expectedPlaylistId
            select pa
        ).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task RemoveTagFromAudiotrack_NoPlaylistNoAudiotrack_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedPlaylistId = DefaultPlaylistId;
        var expectedAudiotrackId = MakeGuid(1);

        // Act
        async Task Action() =>
            await _repository.RemoveAudiotrackFromPlaylist(
                expectedPlaylistId,
                expectedAudiotrackId
            );

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task DeleteByPlaylist_AudiotracksAndPlaylistExist_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedPlaylistId = DefaultPlaylistId;

        await AddDefaultUserWithPlaylist();

        for (byte i = 1; i < 3; ++i)
        {
            var audiotrackId = MakeGuid(i);
            await AddAudiotrackWithId(audiotrackId);
            await context.PlaylistsAudiotracks.AddAsync(
                new(expectedPlaylistId, audiotrackId)
            );
        }
        await context.SaveChangesAsync();

        // Act
        await _repository.DeleteByPlaylist(expectedPlaylistId);

        // Assert
        var actual = (
            from pa in context.PlaylistsAudiotracks
            where pa.PlaylistId == expectedPlaylistId
            select pa
        ).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteByPlaylist_NoPlaylist_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedPlaylistId = DefaultPlaylistId;

        // Act
        await _repository.DeleteByPlaylist(expectedPlaylistId);

        // Assert
        var actual = (
            from pa in context.PlaylistsAudiotracks
            where pa.PlaylistId == expectedPlaylistId
            select pa
        ).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteByAudiotrack_AudiotracksAndTagsExist_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedAudiotrackId = MakeGuid(1);

        await AddDefaultUserWithPlaylist();
        await AddAudiotrackWithId(expectedAudiotrackId);

        for (byte i = 2; i < 4; ++i)
        {
            var playlistId = MakeGuid(i);
            await AddPlaylistWithUserId(playlistId, DefaultUserId);
            await context.PlaylistsAudiotracks.AddAsync(
                new(playlistId, expectedAudiotrackId)
            );
        }
        await context.SaveChangesAsync();

        // Act
        await _repository.DeleteByAudiotrack(expectedAudiotrackId);

        // Assert
        var actual = (
            from pa in context.PlaylistsAudiotracks
            where pa.AudiotrackId == expectedAudiotrackId
            select pa
        ).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteByAudiotrack_NoAudiotrack_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedAudiotrackId = MakeGuid(1);

        // Act
        await _repository.DeleteByAudiotrack(expectedAudiotrackId);

        // Assert
        var actual = (
            from pa in context.PlaylistsAudiotracks
            where pa.AudiotrackId == expectedAudiotrackId
            select pa
        ).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAllAudiotracksFromPlaylist_AudiotracksAndPlaylistExist_ReturnsAudiotracks()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedPlaylistId = DefaultPlaylistId;
        List<Guid> expectedAudiotrackIds = [MakeGuid(1), MakeGuid(2)];

        foreach (var aid in expectedAudiotrackIds)
        {
            await AddAudiotrackWithId(aid);
            await context.PlaylistsAudiotracks.AddAsync(
                new(expectedPlaylistId, aid)
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAllAudiotracksFromPlaylist(
            expectedPlaylistId
        );

        // Assert
        Assert.Equal(2, actual.Count);
        Assert.Equal(expectedAudiotrackIds[0], actual[0].Id);
        Assert.Equal(expectedAudiotrackIds[1], actual[1].Id);
    }

    [Fact]
    public async Task GetAllAudiotracksFromPlaylist_NoAudiotracks_ReturnsEmpty()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedPlaylistId = DefaultPlaylistId;

        // Act
        var actual = await _repository.GetAllAudiotracksFromPlaylist(
            expectedPlaylistId
        );

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task IsAudiotrackInPlaylist_AudiotrackIsInPlaylist_ReturnsTrue()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedPlaylistId = DefaultPlaylistId;
        var expectedAudiotrackId = MakeGuid(1);

        await AddAudiotrackWithId(expectedAudiotrackId);
        await context.PlaylistsAudiotracks.AddAsync(
            new(expectedPlaylistId, expectedAudiotrackId)
        );
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.IsAudiotrackInPlaylist(
            expectedPlaylistId,
            expectedAudiotrackId
        );

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async Task IsAudiotrackInPlaylist_AudiotrackNotInPlaylist_ReturnsFalse()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedPlaylistId = DefaultPlaylistId;
        var expectedAudiotrackId = MakeGuid(1);

        // Act
        var actual = await _repository.IsAudiotrackInPlaylist(
            expectedPlaylistId,
            expectedAudiotrackId
        );

        // Assert
        Assert.False(actual);
    }
}
