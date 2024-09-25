using MewingPad.Common.Exceptions;
using MewingPad.Services.TagService;
using MewingPad.Tests.Factories.Core;

namespace MewingPad.Tests.BusinessLogic.UnitTests.Services;

public class TagServiceUnitTest : BaseServiceTestClass
{
    private readonly TagService _tagService;
    private readonly Mock<ITagRepository> _mockTagRepository = new();
    private readonly Mock<IAudiotrackRepository> _mockAudiotrackRepository =
        new();
    private readonly Mock<ITagAudiotrackRepository> _mockTagAudiotrackRepository =
        new();

    public TagServiceUnitTest()
    {
        _tagService = new TagService(
            _mockTagRepository.Object,
            _mockAudiotrackRepository.Object,
            _mockTagAudiotrackRepository.Object
        );
    }

    [Fact]
    public async Task CreateTag_CreateUnique_Ok()
    {
        // Arrange
        var expectedTag = TagFabric.Create(MakeGuid(1), MakeGuid(1), "Name");
        List<Tag> tags = [];

        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Tag)!);
        _mockTagRepository
            .Setup(s => s.AddTag(It.IsAny<Tag>()))
            .Callback((Tag t) => tags.Add(new(expectedTag)));

        // Act
        await _tagService.CreateTag(expectedTag);

        // Assert
        Assert.Single(tags);
        Assert.Equal(expectedTag, tags[0]);
    }

    [Fact]
    public async Task CreateTag_CreateExistent_Error()
    {
        // Arrange
        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(TagFabric.CreateEmpty());

        // Act
        async Task Action() => await _tagService.CreateTag(new());

        // Assert
        await Assert.ThrowsAsync<TagExistsException>(Action);
    }

    [Fact]
    public async Task AssignTagToAudiotrack_TagAndAudiotrackExist_Ok()
    {
        // Arrange
        var expectedTagId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);
        List<Tuple<Guid, Guid>> tagsAudiotracks = [];

        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(TagFabric.CreateEmpty());
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(AudiotrackFabric.CreateEmpty());
        _mockTagAudiotrackRepository
            .Setup(s =>
                s.AssignTagToAudiotrack(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .Callback(
                (Guid aid, Guid tid) =>
                    tagsAudiotracks.Add(Tuple.Create(aid, tid))
            );

        // Act
        await _tagService.AssignTagToAudiotrack(
            expectedTagId,
            expectedAudiotrackId
        );

        // Assert
        Assert.Single(tagsAudiotracks);
        Assert.Equal(expectedTagId, tagsAudiotracks[0].Item2);
        Assert.Equal(expectedAudiotrackId, tagsAudiotracks[0].Item1);
    }

    public static IEnumerable<object[]> AssignTagToAudiotrack_GetTestData()
    {
        yield return new object[]
        {
            default(Tag)!,
            AudiotrackFabric.CreateEmpty(),
            typeof(TagNotFoundException),
        };

        yield return new object[]
        {
            TagFabric.CreateEmpty(),
            default(Audiotrack)!,
            typeof(AudiotrackNotFoundException),
        };
    }

    [Theory]
    [MemberData(nameof(AssignTagToAudiotrack_GetTestData))]
    public async Task AssignTagToAudiotrack_EntityWithIdNotFound_Error(
        Tag returnedTag,
        Audiotrack returnedAudiotrack,
        Type expectedExceptionType
    )
    {
        // Arrange
        var expectedTagId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);
        List<Tuple<Guid, Guid>> tagsAudiotracks = [];

        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedTag);
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedAudiotrack);
        _mockTagAudiotrackRepository
            .Setup(s =>
                s.AssignTagToAudiotrack(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .Callback(
                (Guid aid, Guid tid) =>
                    tagsAudiotracks.Add(Tuple.Create(aid, tid))
            );

        // Act
        var exception = await Assert.ThrowsAsync(
            expectedExceptionType,
            async () =>
            {
                await _tagService.AssignTagToAudiotrack(
                    expectedTagId,
                    expectedAudiotrackId
                );
            }
        );

        // Assert
        Assert.IsType(expectedExceptionType, exception);
    }

    [Fact]
    public async Task UpdateTagName_TagExists_Ok()
    {
        // Arrange
        var oldTag = TagFabric.Create(MakeGuid(1), MakeGuid(1), "Name");
        List<Tag> tags = [oldTag];
        var expectedTag = new Tag(oldTag) { Name = "New" };

        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(oldTag);
        _mockTagRepository
            .Setup(s => s.UpdateTag(It.IsAny<Tag>()))
            .Callback((Tag t) => tags[0].Name = t.Name);

        // Act
        await _tagService.UpdateTagName(oldTag.Id, expectedTag.Name);

        // Assert
        Assert.Single(tags);
        Assert.Equal(expectedTag.Id, tags[0].Id);
        Assert.Equal(expectedTag.Name, tags[0].Name);
        Assert.Equal(expectedTag.AuthorId, tags[0].AuthorId);
    }

    [Fact]
    public async Task UpdateTagName_UpdateNonexistent_Error()
    {
        // Arrange
        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Tag)!);

        // Act
        async Task Action() => await _tagService.UpdateTagName(new(), "");

        // Assert
        await Assert.ThrowsAsync<TagNotFoundException>(Action);
    }

    [Fact]
    public async Task DeleteTag_TagExists_Ok()
    {
        // Arrange
        var expectedTag = TagFabric.Create(MakeGuid(1), MakeGuid(1), "Name");
        List<Tag> tags = [expectedTag];

        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(expectedTag);
        _mockTagRepository
            .Setup(s => s.DeleteTag(It.IsAny<Guid>()))
            .Callback((Guid id) => tags.Remove(expectedTag));

        // Act
        await _tagService.DeleteTag(expectedTag.Id);

        // Assert
        Assert.Empty(tags);
    }

    [Fact]
    public async Task DeleteTag_DeleteNonexistent_Error()
    {
        // Arrange
        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Tag)!);

        // Act
        async Task Action() => await _tagService.DeleteTag(new());

        // Assert
        await Assert.ThrowsAsync<TagNotFoundException>(Action);
    }

    [Fact]
    public async Task DeleteTagFromAudiotrack_TagAndAudiotrackExist_Ok()
    {
        // Arrange
        var expectedTagId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);
        List<Tuple<Guid, Guid>> tagsAudiotracks =
        [
            Tuple.Create(expectedTagId, expectedAudiotrackId),
        ];

        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(TagFabric.CreateEmpty());
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(AudiotrackFabric.CreateEmpty());
        _mockTagAudiotrackRepository
            .Setup(s =>
                s.RemoveTagFromAudiotrack(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .Callback(
                (Guid aid, Guid tid) =>
                    tagsAudiotracks.Remove(Tuple.Create(tid, aid))
            );

        // Act
        await _tagService.DeleteTagFromAudiotrack(
            expectedTagId,
            expectedAudiotrackId
        );

        // Assert
        Assert.Empty(tagsAudiotracks);
    }

    public static IEnumerable<object[]> DeleteTagFromAudiotrack_GetTestData()
    {
        yield return new object[]
        {
            default(Tag)!,
            AudiotrackFabric.CreateEmpty(),
            typeof(TagNotFoundException),
        };

        yield return new object[]
        {
            TagFabric.CreateEmpty(),
            default(Audiotrack)!,
            typeof(AudiotrackNotFoundException),
        };
    }

    [Theory]
    [MemberData(nameof(DeleteTagFromAudiotrack_GetTestData))]
    public async Task DeleteTagFromAudiotrack_EntityWithIdNotFound_Error(
        Tag returnedTag,
        Audiotrack returnedAudiotrack,
        Type expectedExceptionType
    )
    {
        // Arrange
        var expectedTagId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);
        List<Tuple<Guid, Guid>> tagsAudiotracks =
        [
            Tuple.Create(expectedTagId, expectedAudiotrackId),
        ];

        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedTag);
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(returnedAudiotrack);
        _mockTagAudiotrackRepository
            .Setup(s =>
                s.RemoveTagFromAudiotrack(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .Callback(
                (Guid aid, Guid tid) =>
                    tagsAudiotracks.Add(Tuple.Create(aid, tid))
            );

        // Act
        var exception = await Assert.ThrowsAsync(
            expectedExceptionType,
            async () =>
            {
                await _tagService.AssignTagToAudiotrack(
                    expectedTagId,
                    expectedAudiotrackId
                );
            }
        );

        // Assert
        Assert.IsType(expectedExceptionType, exception);
    }

    public static IEnumerable<object[]> GetAudiotrackTags_GetTestData()
    {
        yield return new object[]
        {
            new List<Tag>([TagFabric.Create(MakeGuid(1), MakeGuid(1), "Name")]),
            1,
        };

        yield return new object[] { new List<Tag>([]), 0 };
    }

    [Theory]
    [MemberData(nameof(GetAudiotrackTags_GetTestData))]
    public async Task GetAudiotrackTags_AudiotrackExists_ReturnsTags(
        List<Tag> expectedTags,
        int expectedCount
    )
    {
        // Arrange
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(AudiotrackFabric.CreateEmpty());
        _mockTagAudiotrackRepository
            .Setup(s => s.GetAudiotrackTags(It.IsAny<Guid>()))
            .ReturnsAsync(expectedTags);

        // Act
        var actual = await _tagService.GetAudiotrackTags(new());

        // Assert
        Assert.Equal(expectedCount, expectedTags.Count);
    }

    [Fact]
    public async Task GetAudiotrackTags_NoAudiotrackWithId_Error()
    {
        // Arrange
        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Audiotrack)!);

        // Act
        async Task Action() => await _tagService.GetAudiotrackTags(new());

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }

    public static IEnumerable<object[]> GetAllTags_GetTestData()
    {
        yield return new object[]
        {
            new List<Tag>([TagFabric.Create(MakeGuid(1), MakeGuid(1), "Name")]),
            1,
        };

        yield return new object[] { new List<Tag>([]), 0 };
    }

    [Theory]
    [MemberData(nameof(GetAllTags_GetTestData))]
    public async Task GetAllTags_ReturnsTags(
        List<Tag> expectedTags,
        int expectedCount
    )
    {
        // Arrange
        _mockTagRepository
            .Setup(s => s.GetAllTags())
            .ReturnsAsync(expectedTags);

        // Act
        var actual = await _tagService.GetAllTags();

        // Assert
        Assert.Equal(expectedCount, expectedTags.Count);
    }

    public static IEnumerable<object[]> GetAudiotracksWithTags_GetTestData()
    {
        yield return new object[]
        {
            new List<Guid>([MakeGuid(1)]),
            new List<Audiotrack>(
                [
                    AudiotrackFabric.Create(
                        MakeGuid(1),
                        "Title",
                        MakeGuid(1),
                        "file.mp3"
                    ),
                ]
            ),
            1,
        };

        yield return new object[]
        {
            new List<Guid>([]),
            new List<Audiotrack>([]),
            0,
        };
    }

    [Theory]
    [MemberData(nameof(GetAudiotracksWithTags_GetTestData))]
    public async Task GetAudiotracksWithTags_TagsExist_ReturnsAudiotracks(
        List<Guid> requestTagIds,
        List<Audiotrack> expectedAudiotracks,
        int expectedCount
    )
    {
        // Arrange
        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(TagFabric.CreateEmpty());
        _mockTagAudiotrackRepository
            .Setup(s => s.GetAudiotracksWithTags(It.IsAny<List<Guid>>()))
            .ReturnsAsync(expectedAudiotracks);

        // Act
        var actual = await _tagService.GetAudiotracksWithTags(requestTagIds);

        // Assert
        Assert.Equal(expectedCount, expectedAudiotracks.Count);
    }

    [Fact]
    public async Task GetAudiotracksWithTags_TagNonexstent_Error()
    {
        // Arrange
        _mockTagRepository
            .Setup(s => s.GetTagById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Tag)!);

        // Act
        async Task Action() => await _tagService.GetAudiotracksWithTags([new()]);

        // Assert
        await Assert.ThrowsAsync<TagNotFoundException>(Action);
    }
}
