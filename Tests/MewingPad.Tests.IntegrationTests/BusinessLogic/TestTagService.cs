using MewingPad.Common.Exceptions;
using MewingPad.Database.Context;
using MewingPad.Database.Models;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Services.TagService;
using MewingPad.Tests.Factories.Core;

namespace MewingPad.Tests.IntegrationTests.BusinessLogic;

[Collection("Test Database")]
public class TagServiceIntegrationTest : BaseServiceTestClass
{
    private readonly MewingPadDbContext _context;
    private readonly TagService _service;
    private readonly TagRepository _tagRepository;
    private readonly AudiotrackRepository _audiotrackRepository;
    private readonly TagAudiotrackRepository _tagAudiotrackRepository;

    private readonly Guid DefaultTagId = MakeGuid(1);

    private async Task AddDefaultTag()
    {
        await _context.Tags.AddAsync(
            new TagDbModelBuilder()
                .WithId(DefaultTagId)
                .WithAuthorId(DefaultUserId)
                .WithName("Name")
                .Build()
        );
        await _context.SaveChangesAsync();
    }

    public TagServiceIntegrationTest(DatabaseFixture fixture)
        : base(fixture)
    {
        _context = Fixture.CreateContext();

        _tagRepository = new(_context);
        _audiotrackRepository = new(_context);
        _tagAudiotrackRepository = new(_context);

        _service = new(
            _tagRepository,
            _audiotrackRepository,
            _tagAudiotrackRepository
        );
    }

    [Fact]
    public async Task CreateTag_CreateUnique_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var tag = TagFabric.Create(DefaultTagId, DefaultUserId, "Name");

        // Act
        await _service.CreateTag(tag);

        // Assert
        var actual = (from t in _context.Tags select t).ToList();
        Assert.Single(actual);
        Assert.Equal(tag.Id, actual[0].Id);
    }

    [Fact]
    public async Task CreateTag_CreateExistent_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultTag();

        var tag = TagFabric.Create(DefaultTagId, DefaultUserId, "Name");

        // Act
        async Task Action() => await _service.CreateTag(tag);

        // Assert
        await Assert.ThrowsAsync<TagExistsException>(Action);
    }

    [Fact]
    public async Task AssignTagToAudiotrack_TagAndAudiotrackExist_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultTag();

        var expectedTagId = DefaultTagId;
        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act
        await _service.AssignTagToAudiotrack(
            expectedTagId,
            expectedAudiotrackId
        );

        // Assert
        var actual = (from ta in _context.TagsAudiotracks select ta).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedTagId, actual[0].TagId);
        Assert.Equal(expectedAudiotrackId, actual[0].AudiotrackId);
    }

    [Fact]
    public async Task AssignTagToAudiotrack_AudiotrackWithIdNotFound_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultTag();

        var expectedTagId = DefaultTagId;
        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act
        async Task Action() =>
            await _service.AssignTagToAudiotrack(
                expectedTagId,
                expectedAudiotrackId
            );

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    [Fact]
    public async Task AssignTagToAudiotrack_TagWithIdNotFound_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedTagId = DefaultTagId;
        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act
        async Task Action() =>
            await _service.AssignTagToAudiotrack(
                expectedTagId,
                expectedAudiotrackId
            );

        // Assert
        await Assert.ThrowsAsync<TagNotFoundException>(Action);
    }

    [Fact]
    public async Task UpdateTagName_TagExists_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultTag();

        var expectedTagId = DefaultTagId;
        const string expectedName = "AAA";

        // Act
        await _service.UpdateTagName(expectedTagId, expectedName);

        // Assert
        var actual = (from t in _context.Tags select t).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedTagId, actual[0].Id);
        Assert.Equal(expectedName, actual[0].Name);
    }

    [Fact]
    public async Task UpdateTagName_UpdateNonexistent_Error()
    {
        // Arrange

        // Act
        async Task Action() => await _service.UpdateTagName(new(), "");

        // Assert
        await Assert.ThrowsAsync<TagNotFoundException>(Action);
    }

    [Fact]
    public async Task DeleteTag_TagExists_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultTag();

        // Act
        await _service.DeleteTag(DefaultTagId);

        // Assert
        var actual = (from t in _context.Tags select t).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTag_DeleteNonexistent_Error()
    {
        // Arrange

        // Act
        async Task Action() => await _service.DeleteTag(new());

        // Assert
        await Assert.ThrowsAsync<TagNotFoundException>(Action);
    }

    [Fact]
    public async Task DeleteTagFromAudiotrack_TagAndAudiotrackExist_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultTag();

        var expectedTagId = DefaultTagId;
        var expectedAudiotrackId = DefaultAudiotrackId;

        using (var context = Fixture.CreateContext())
        {
            await context.AddAsync(
                new TagAudiotrackDbModel(expectedTagId, expectedAudiotrackId)
            );
            await context.SaveChangesAsync();
        }

        // Act
        await _service.DeleteTagFromAudiotrack(
            expectedTagId,
            expectedAudiotrackId
        );

        // Assert
        var actual = (from ta in _context.TagsAudiotracks select ta).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTagFromAudiotrack_AudiotrackWithIdNotFound_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultTag();

        var expectedTagId = DefaultTagId;
        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act

        async Task Action() =>
            await _service.AssignTagToAudiotrack(
                expectedTagId,
                expectedAudiotrackId
            );

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    [Fact]
    public async Task DeleteTagFromAudiotrack_TagkWithIdNotFound_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedTagId = DefaultTagId;
        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act
        async Task Action() =>
            await _service.AssignTagToAudiotrack(
                expectedTagId,
                expectedAudiotrackId
            );

        // Assert
        await Assert.ThrowsAsync<TagNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAudiotrackTags_AudiotrackExists_ReturnsTags()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultTag();

        var expectedTagId = DefaultTagId;
        var expectedAudiotrackId = DefaultAudiotrackId;

        using (var context = Fixture.CreateContext())
        {
            await context.AddAsync(
                new TagAudiotrackDbModel(expectedTagId, expectedAudiotrackId)
            );
            await context.SaveChangesAsync();
        }

        // Act
        var actual = await _service.GetAudiotrackTags(expectedAudiotrackId);

        // Assert
        Assert.Single(actual);
        Assert.Equal(expectedTagId, actual[0].Id);
    }

    [Fact]
    public async Task GetAudiotrackTags_AudiotrackExists_ReturnsEmpty()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultTag();

        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act
        var actual = await _service.GetAudiotrackTags(expectedAudiotrackId);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotrackTags_NoAudiotrackWithId_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultTag();

        // Act
        async Task Action() =>
            await _service.GetAudiotrackTags(DefaultAudiotrackId);

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAllTags_TagsExist_ReturnsTags()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultTag();

        // Act
        var actual = await _service.GetAllTags();

        // Assert
        Assert.Single(actual);
    }

    [Fact]
    public async Task GetAllTags_NoTags_ReturnsTags()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        // Act
        var actual = await _service.GetAllTags();

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotracksWithTags_TagsExist_ReturnsAudiotracks()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var requestTagIds = new List<Guid>([MakeGuid(1), MakeGuid(2)]);

        using (var context = Fixture.CreateContext())
        {
            await context.Tags.AddRangeAsync(
                [
                    new TagDbModelBuilder()
                        .WithId(requestTagIds[0])
                        .WithAuthorId(DefaultUserId)
                        .WithName("Name1")
                        .Build(),
                    new TagDbModelBuilder()
                        .WithId(requestTagIds[1])
                        .WithAuthorId(DefaultUserId)
                        .WithName("Name2")
                        .Build(),
                ]
            );
            await context.AddRangeAsync(
                [
                    new TagAudiotrackDbModel(
                        requestTagIds[0],
                        DefaultAudiotrackId
                    ),
                    new TagAudiotrackDbModel(
                        requestTagIds[1],
                        DefaultAudiotrackId
                    ),
                ]
            );
            await context.SaveChangesAsync();
        }

        // Act
        var actual = await _service.GetAudiotracksWithTags(requestTagIds);

        // Assert
        Assert.Single(actual);
    }

    [Fact]
    public async Task GetAudiotracksWithTags_TagNonexstent_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        // Act
        async Task Action() =>
            await _service.GetAudiotracksWithTags([new()]);

        // Assert
        await Assert.ThrowsAsync<TagNotFoundException>(Action);
    }
}
