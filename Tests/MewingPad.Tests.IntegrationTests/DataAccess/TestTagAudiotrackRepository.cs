using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;


namespace MewingPad.Tests.IntegrationTests.DataAccess;

[Collection("Test Database")]
public class TestTagAudiotrackRepository : BaseRepositoryTestClass
{
    private TagAudiotrackRepository _repository;

    public TestTagAudiotrackRepository(DatabaseFixture fixture)
        : base(fixture)
    {
        _repository = new(Fixture.CreateContext());
    }

    private async Task AddTagWithAuthor(Guid tagId, Guid authorId)
    {
        using var context = Fixture.CreateContext();

        var tagDbo = new TagDbModelBuilder()
            .WithId(tagId)
            .WithAuthorId(authorId)
            .WithName($"Name_{tagId}")
            .Build();
        await context.Tags.AddAsync(tagDbo);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task AssignTagToAudiotrack_AssignUnique_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedTagId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);

        await AddDefaultUserWithPlaylist();
        await AddTagWithAuthor(expectedTagId, DefaultUserId);
        await AddAudiotrackWithId(expectedAudiotrackId);

        // Act
        await _repository.AssignTagToAudiotrack(
            expectedAudiotrackId,
            expectedTagId
        );

        // Assert
        var actual = (
            from ta in context.TagsAudiotracks
            where ta.TagId == expectedTagId
            select ta
        ).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedTagId, actual[0].TagId);
        Assert.Equal(expectedAudiotrackId, actual[0].AudiotrackId);
    }

    [Fact]
    public async Task AssignTagToAudiotrack_AssignExisiting_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedTagId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);

        await AddDefaultUserWithPlaylist();
        await AddTagWithAuthor(expectedTagId, DefaultUserId);
        await AddAudiotrackWithId(expectedAudiotrackId);
        await context.TagsAudiotracks.AddAsync(
            new(expectedTagId, expectedAudiotrackId)
        );
        await context.SaveChangesAsync();

        // Act
        async Task Action() =>
            await _repository.AssignTagToAudiotrack(
                expectedAudiotrackId,
                expectedTagId
            );

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task RemoveTagFromAudiotrack_AudiotrackAndTagExist_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedTagId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);

        await AddDefaultUserWithPlaylist();
        await AddTagWithAuthor(expectedTagId, DefaultUserId);

        await AddAudiotrackWithId(expectedAudiotrackId);
        await context.TagsAudiotracks.AddAsync(
            new(expectedTagId, expectedAudiotrackId)
        );
        await context.SaveChangesAsync();

        // Act
        await _repository.RemoveTagFromAudiotrack(
            expectedAudiotrackId,
            expectedTagId
        );

        // Assert
        var actual = (
            from ta in context.TagsAudiotracks
            where ta.TagId == expectedTagId
            select ta
        ).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task RemoveTagFromAudiotrack_NoTagNoAudiotrack_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedTagId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);

        // Act
        async Task Action() =>
            await _repository.RemoveTagFromAudiotrack(
                expectedTagId,
                expectedAudiotrackId
            );

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task DeleteByTag_AudiotracksAndTagsExist_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedTagId = MakeGuid(1);

        await AddDefaultUserWithPlaylist();
        await AddTagWithAuthor(expectedTagId, DefaultUserId);

        for (byte i = 1; i < 3; ++i)
        {
            var audiotrackId = MakeGuid(i);
            await AddAudiotrackWithId(audiotrackId);
            await context.TagsAudiotracks.AddAsync(
                new(expectedTagId, audiotrackId)
            );
        }
        await context.SaveChangesAsync();

        // Act
        await _repository.DeleteByTag(expectedTagId);

        // Assert
        var actual = (
            from ta in context.TagsAudiotracks
            where ta.TagId == expectedTagId
            select ta
        ).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteByTag_NoTagsNoAudiotracks_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedTagId = MakeGuid(1);

        // Act
        await _repository.DeleteByTag(expectedTagId);

        // Assert
        var actual = (
            from ta in context.TagsAudiotracks
            where ta.TagId == expectedTagId
            select ta
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

        for (byte i = 1; i < 3; ++i)
        {
            var tagId = MakeGuid(i);
            await AddTagWithAuthor(tagId, DefaultUserId);
            await context.TagsAudiotracks.AddAsync(
                new(tagId, expectedAudiotrackId)
            );
        }
        await context.SaveChangesAsync();

        // Act
        await _repository.DeleteByAudiotrack(expectedAudiotrackId);

        // Assert
        var actual = (
            from ta in context.TagsAudiotracks
            where ta.AudiotrackId == expectedAudiotrackId
            select ta
        ).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteByAudiotrack_NoTagsNoAudiotracks_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedAudiotrackId = MakeGuid(1);

        // Act
        await _repository.DeleteByAudiotrack(expectedAudiotrackId);

        // Assert
        var actual = (
            from ta in context.TagsAudiotracks
            where ta.AudiotrackId == expectedAudiotrackId
            select ta
        ).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotracksWithTags_AudiotracksAndTagsExist_ReturnsAudiotracks()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        List<Guid> expectedTagIds = [MakeGuid(1), MakeGuid(2)];
        List<Guid> expectedAudiotrackIds = [MakeGuid(1), MakeGuid(2)];

        foreach (
            var (tid, aid) in expectedTagIds.Zip(
                expectedAudiotrackIds,
                (k, v) => ValueTuple.Create(k, v)
            )
        )
        {
            await AddAudiotrackWithId(aid);
            await AddTagWithAuthor(tid, DefaultUserId);
            await context.TagsAudiotracks.AddAsync(new(tid, aid));
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotracksWithTags(expectedTagIds);

        // Assert
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public async Task GetAudiotracksWithTags_NoAudiotracks_ReturnsEmpty()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        List<Guid> expectedTagIds = [MakeGuid(1), MakeGuid(2)];
        foreach (var tid in expectedTagIds)
        {
            await AddTagWithAuthor(tid, DefaultUserId);
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotracksWithTags(expectedTagIds);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotracksWithTags_NoTags_ReturnsEmpty()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        List<Guid> expectedAudiotrackIds = [MakeGuid(1), MakeGuid(2)];
        List<Guid> expectedTagIds = [MakeGuid(1), MakeGuid(2)];

        foreach (var aid in expectedAudiotrackIds)
        {
            await AddAudiotrackWithId(aid);
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotracksWithTags(expectedTagIds);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotracksWithTags_EmptyTagsInput_ReturnsEmpty()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        List<Guid> expectedTagIds = [MakeGuid(1), MakeGuid(2)];
        List<Guid> expectedAudiotrackIds = [MakeGuid(1), MakeGuid(2)];

        foreach (
            var (tid, aid) in expectedTagIds.Zip(
                expectedAudiotrackIds,
                (k, v) => ValueTuple.Create(k, v)
            )
        )
        {
            await AddAudiotrackWithId(aid);
            await AddTagWithAuthor(tid, DefaultUserId);
            await context.TagsAudiotracks.AddAsync(new(tid, aid));
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotracksWithTags([]);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotrackTags_AudiotrackAndTagsExist_ReturnsTags()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedAudiotrackId = DefaultAudiotrackId;
        await AddAudiotrackWithId(expectedAudiotrackId);

        List<Guid> expectedTagIds = [MakeGuid(1), MakeGuid(2)];

        foreach (var tid in expectedTagIds)
        {
            await AddTagWithAuthor(tid, DefaultUserId);
            await context.TagsAudiotracks.AddAsync(
                new(tid, expectedAudiotrackId)
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotrackTags(expectedAudiotrackId);

        // Assert
        Assert.Equal(2, actual.Count);
        Assert.Equal(expectedTagIds[0], actual[0].Id);
        Assert.Equal(expectedTagIds[1], actual[1].Id);
    }

    [Fact]
    public async Task GetAudiotrackTags_AudiotrackHasNoTags_ReturnsEmpty()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act
        var actual = await _repository.GetAudiotrackTags(expectedAudiotrackId);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotrackTags_NoAudiotrackWithId_ReturnsEmpty()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act
        var actual = await _repository.GetAudiotrackTags(expectedAudiotrackId);

        // Assert
        Assert.Empty(actual);
    }
}
