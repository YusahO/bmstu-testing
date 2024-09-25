using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using Moq.EntityFrameworkCore;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

public class TestTagAudiotrackRepository : BaseRepositoryTestClass
{
    private TagAudiotrackRepository _repository;
    private readonly MockDbContextFactory _mockFactory;

    public TestTagAudiotrackRepository()
    {
        _mockFactory = new MockDbContextFactory();
        _repository = new(_mockFactory.MockContext.Object);
    }

    [Fact]
    public async Task AssignTagToAudiotrack_AssignUnique_Ok()
    {
        // Arrange
        var expectedTagId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);

        List<Tuple<Guid, Guid>> pairs = [];
        _mockFactory
            .MockTagsAudiotracksDbModel.Setup(s =>
                s.AddAsync(It.IsAny<TagAudiotrackDbModel>(), default)
            )
            .Callback<TagAudiotrackDbModel, CancellationToken>(
                (ta, token) =>
                    pairs.Add(Tuple.Create(ta.TagId, ta.AudiotrackId))
            );

        // Act
        await _repository.AssignTagToAudiotrack(
            expectedAudiotrackId,
            expectedTagId
        );

        // Assert
        Assert.Single(pairs);
        Assert.Equal(expectedTagId, pairs[0].Item1);
        Assert.Equal(expectedAudiotrackId, pairs[0].Item2);
    }

    [Fact]
    public async Task AssignTagToAudiotrack_AssignExisiting_Error()
    {
        // Arrange
        _mockFactory
            .MockTagsAudiotracksDbModel.Setup(s =>
                s.AddAsync(It.IsAny<TagAudiotrackDbModel>(), default)
            )
            .Callback<TagAudiotrackDbModel, CancellationToken>(
                (a, token) => throw new RepositoryException()
            );

        // Act
        async Task Action() =>
            await _repository.AssignTagToAudiotrack(new(), new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task RemoveTagFromAudiotrack_AudiotrackAndTagExist_Ok()
    {
        // Arrange
        var expectedTagId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);

        List<Tuple<Guid, Guid>> pairs =
        [
            Tuple.Create(expectedTagId, expectedAudiotrackId),
        ];

        _mockFactory
            .MockTagsAudiotracksDbModel.Setup(s =>
                s.Remove(It.IsAny<TagAudiotrackDbModel>())
            )
            .Callback<TagAudiotrackDbModel>(
                (ta) => pairs.Remove(Tuple.Create(ta.TagId, ta.AudiotrackId))
            );

        // Act
        await _repository.RemoveTagFromAudiotrack(
            expectedAudiotrackId,
            expectedTagId
        );

        // Assert
        Assert.Empty(pairs);
    }

    [Fact]
    public async Task RemoveTagFromAudiotrack_NoTagNoAudiotrack_Error()
    {
        // Arrange
        _mockFactory
            .MockTagsAudiotracksDbModel.Setup(s =>
                s.Remove(It.IsAny<TagAudiotrackDbModel>())
            )
            .Callback<TagAudiotrackDbModel>(
                (ta) => throw new RepositoryException()
            );

        // Act
        async Task Action() =>
            await _repository.RemoveTagFromAudiotrack(new(), new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    public static IEnumerable<object[]> DeleteByTag_GetTestData()
    {
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(),
            new List<TagAudiotrackDbModel>(),
            new List<Tuple<Guid, Guid>>(),
        };
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(
                [Tuple.Create(MakeGuid(1), MakeGuid(1))]
            ),
            new List<TagAudiotrackDbModel>([new(MakeGuid(1), MakeGuid(1))]),
            new List<Tuple<Guid, Guid>>(),
        };
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(
                [Tuple.Create(MakeGuid(2), MakeGuid(2))]
            ),
            new List<TagAudiotrackDbModel>([new(MakeGuid(2), MakeGuid(2))]),
            new List<Tuple<Guid, Guid>>(
                [Tuple.Create(MakeGuid(2), MakeGuid(2))]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(DeleteByTag_GetTestData))]
    public async Task DeleteByTag_Ok(
        List<Tuple<Guid, Guid>> initial,
        List<TagAudiotrackDbModel> returnedPairs,
        List<Tuple<Guid, Guid>> expectedPairs
    )
    {
        // Arrange
        List<Tuple<Guid, Guid>> actual = initial;

        _mockFactory.MockTagsAudiotracksDbModel =
            MockDbContextFactory.SetupMockDbSet(returnedPairs);

        _mockFactory
            .MockContext.Setup(m => m.TagsAudiotracks)
            .ReturnsDbSet(_mockFactory.MockTagsAudiotracksDbModel.Object);
        _mockFactory
            .MockContext.Setup(s =>
                s.TagsAudiotracks.RemoveRange(
                    It.IsAny<IEnumerable<TagAudiotrackDbModel>>()
                )
            )
            .Callback(
                (IEnumerable<TagAudiotrackDbModel> ta) =>
                {
                    foreach (var e in ta)
                    {
                        actual.Remove(Tuple.Create(e.TagId, e.AudiotrackId));
                    }
                }
            );

        // Act
        await _repository.DeleteByTag(MakeGuid(1));

        // Assert
        Assert.Equal(expectedPairs, actual);
    }

    public static IEnumerable<object[]> DeleteByAudiotrack_GetTestData()
    {
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(),
            new List<TagAudiotrackDbModel>(),
            new List<Tuple<Guid, Guid>>(),
        };
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(
                [Tuple.Create(MakeGuid(1), MakeGuid(1))]
            ),
            new List<TagAudiotrackDbModel>([new(MakeGuid(1), MakeGuid(1))]),
            new List<Tuple<Guid, Guid>>(),
        };
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(
                [Tuple.Create(MakeGuid(2), MakeGuid(2))]
            ),
            new List<TagAudiotrackDbModel>([new(MakeGuid(2), MakeGuid(2))]),
            new List<Tuple<Guid, Guid>>(
                [Tuple.Create(MakeGuid(2), MakeGuid(2))]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(DeleteByAudiotrack_GetTestData))]
    public async Task DeleteByAudiotrack_Ok(
        List<Tuple<Guid, Guid>> initial,
        List<TagAudiotrackDbModel> returnedPairs,
        List<Tuple<Guid, Guid>> expectedPairs
    )
    {
        // Arrange
        List<Tuple<Guid, Guid>> actual = initial;

        _mockFactory.MockTagsAudiotracksDbModel =
            MockDbContextFactory.SetupMockDbSet(returnedPairs);

        _mockFactory
            .MockContext.Setup(m => m.TagsAudiotracks)
            .ReturnsDbSet(_mockFactory.MockTagsAudiotracksDbModel.Object);
        _mockFactory
            .MockContext.Setup(s =>
                s.TagsAudiotracks.RemoveRange(
                    It.IsAny<IEnumerable<TagAudiotrackDbModel>>()
                )
            )
            .Callback(
                (IEnumerable<TagAudiotrackDbModel> ta) =>
                {
                    foreach (var e in ta)
                    {
                        actual.Remove(Tuple.Create(e.TagId, e.AudiotrackId));
                    }
                }
            );

        // Act
        await _repository.DeleteByAudiotrack(MakeGuid(1));

        // Assert
        Assert.Equal(expectedPairs, actual);
    }

    public static IEnumerable<object[]> GetAudiotracksWithTags_GetTestData()
    {
        yield return new object[] { new List<TagAudiotrackDbModel>(), 0 };
        yield return new object[]
        {
            new List<TagAudiotrackDbModel>(
                [new(MakeGuid(1), MakeGuid(1)), new(MakeGuid(2), MakeGuid(1))]
            ),
            1,
        };
        // yield return new object[]
        // {
        //     new List<TagAudiotrackDbModel>(
        //         [new(MakeGuid(1), MakeGuid(1)), new(MakeGuid(2), MakeGuid(2))]
        //     ),
        //     2,
        // };
        yield return new object[]
        {
            new List<TagAudiotrackDbModel>(
                [new(MakeGuid(3), MakeGuid(1)), new(MakeGuid(3), MakeGuid(2))]
            ),
            0,
        };
    }

    [Theory]
    [MemberData(nameof(GetAudiotracksWithTags_GetTestData))]
    public async Task GetAudiotracksWithTags_Found(
        List<TagAudiotrackDbModel> returnedPairs,
        int expectedCount
    )
    {
        // Arrange
        List<Guid> tagIds = [MakeGuid(1), MakeGuid(2)];

        _mockFactory
            .MockContext.Setup(m => m.TagsAudiotracks)
            .ReturnsDbSet(returnedPairs);

        // Act
        var actual = await _repository.GetAudiotracksWithTags(tagIds);

        // Assert
        Assert.Equal(expectedCount, actual.Count);
    }

    public static IEnumerable<object[]> GetAudiotrackTags_GetTestData()
    {
        yield return new object[] { new List<TagAudiotrackDbModel>(), 0 };
        yield return new object[]
        {
            new List<TagAudiotrackDbModel>(
                [new(MakeGuid(1), MakeGuid(1)), new(MakeGuid(2), MakeGuid(1))]
            ),
            2,
        };
    }

    [Theory]
    [MemberData(nameof(GetAudiotrackTags_GetTestData))]
    public async Task GetAudiotrackTags_ReturnsFound(
        List<TagAudiotrackDbModel> returnedPairs,
        int expectedCount
    )
    {
        // Arrange
        var expectedAudiotrackId = MakeGuid(1);

        _mockFactory
            .MockContext.Setup(m => m.TagsAudiotracks)
            .ReturnsDbSet(returnedPairs);

        // Act
        var actual = await _repository.GetAudiotrackTags(expectedAudiotrackId);

        // Assert
        Assert.Equal(expectedCount, actual.Count);
    }
}
