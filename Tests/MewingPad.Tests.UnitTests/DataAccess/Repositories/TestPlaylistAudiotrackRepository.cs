using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using Moq.EntityFrameworkCore;

namespace MewingPad.Tests.UnitTests.DataAccess.Repositories;

public class TestPlaylistAudiotrackRepository : BaseRepositoryTestClass
{
    private PlaylistAudiotrackRepository _repository;
    private readonly MockDbContextFactory _mockFactory;

    public TestPlaylistAudiotrackRepository()
    {
        _mockFactory = new MockDbContextFactory();
        _repository = new(_mockFactory.MockContext.Object);
    }

    [Fact]
    public async Task AddAudiotrackToPlaylist_AddUnique_Ok()
    {
        // Arrange
        var expectedPlaylistId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);

        List<Tuple<Guid, Guid>> pairs = [];
        _mockFactory
            .MockPlaylistsAudiotracksDbModel.Setup(s =>
                s.AddAsync(It.IsAny<PlaylistAudiotrackDbModel>(), default)
            )
            .Callback<PlaylistAudiotrackDbModel, CancellationToken>(
                (pa, token) =>
                    pairs.Add(Tuple.Create(pa.PlaylistId, pa.AudiotrackId))
            );

        // Act
        await _repository.AddAudiotrackToPlaylist(
            expectedPlaylistId,
            expectedAudiotrackId
        );

        // Assert
        Assert.Single(pairs);
        Assert.Equal(expectedPlaylistId, pairs[0].Item1);
        Assert.Equal(expectedAudiotrackId, pairs[0].Item2);
    }

    [Fact]
    public async Task AddAudiotrackToPlaylist_AddExisting_Error()
    {
        // Arrange
        _mockFactory
            .MockPlaylistsAudiotracksDbModel.Setup(s =>
                s.AddAsync(It.IsAny<PlaylistAudiotrackDbModel>(), default)
            )
            .Callback<PlaylistAudiotrackDbModel, CancellationToken>(
                (a, token) => throw new RepositoryException()
            );

        // Act
        async Task Action() =>
            await _repository.AddAudiotrackToPlaylist(new(), new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task RemoveAudiotrackFromPlaylist_AudiotrackAndPlaylistExist_Ok()
    {
        // Arrange
        var expectedPlaylistId = MakeGuid(1);
        var expectedAudiotrackId = MakeGuid(1);

        List<Tuple<Guid, Guid>> pairs =
        [
            Tuple.Create(expectedPlaylistId, expectedAudiotrackId),
        ];

        _mockFactory
            .MockPlaylistsAudiotracksDbModel.Setup(s =>
                s.Remove(It.IsAny<PlaylistAudiotrackDbModel>())
            )
            .Callback<PlaylistAudiotrackDbModel>(
                (ta) =>
                    pairs.Remove(Tuple.Create(ta.PlaylistId, ta.AudiotrackId))
            );

        // Act
        await _repository.RemoveAudiotrackFromPlaylist(
            expectedPlaylistId,
            expectedAudiotrackId
        );

        // Assert
        Assert.Empty(pairs);
    }

    [Fact]
    public async Task RemovePlaylistFromAudiotrack_NoPlaylistNoAudiotrack_Error()
    {
        // Arrange
        _mockFactory
            .MockPlaylistsAudiotracksDbModel.Setup(s =>
                s.Remove(It.IsAny<PlaylistAudiotrackDbModel>())
            )
            .Callback<PlaylistAudiotrackDbModel>(
                (ta) => throw new RepositoryException()
            );

        // Act
        async Task Action() =>
            await _repository.RemoveAudiotrackFromPlaylist(new(), new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    public static IEnumerable<object[]> DeleteByPlaylist_GetTestData()
    {
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(),
            new List<PlaylistAudiotrackDbModel>(),
            new List<Tuple<Guid, Guid>>(),
        };
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>([new(MakeGuid(1), MakeGuid(1))]),
            new List<PlaylistAudiotrackDbModel>(
                [new(MakeGuid(1), MakeGuid(1))]
            ),
            new List<Tuple<Guid, Guid>>(),
        };
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(
                [Tuple.Create(MakeGuid(2), MakeGuid(2))]
            ),
            new List<PlaylistAudiotrackDbModel>(
                [new(MakeGuid(2), MakeGuid(2))]
            ),
            new List<Tuple<Guid, Guid>>(
                [new(MakeGuid(2), MakeGuid(2))]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(DeleteByPlaylist_GetTestData))]
    public async Task DeleteByPlaylist_Ok(
        List<Tuple<Guid, Guid>> initial,
        List<PlaylistAudiotrackDbModel> returnedPairs,
        List<Tuple<Guid, Guid>> expectedPairs
    )
    {
        // Arrange
        List<Tuple<Guid, Guid>> actual = initial;

        _mockFactory.MockPlaylistsAudiotracksDbModel =
            MockDbContextFactory.SetupMockDbSet(returnedPairs);

        _mockFactory
            .MockContext.Setup(m => m.PlaylistsAudiotracks)
            .ReturnsDbSet(_mockFactory.MockPlaylistsAudiotracksDbModel.Object);
        _mockFactory
            .MockContext.Setup(s =>
                s.PlaylistsAudiotracks.RemoveRange(
                    It.IsAny<IEnumerable<PlaylistAudiotrackDbModel>>()
                )
            )
            .Callback(
                (IEnumerable<PlaylistAudiotrackDbModel> ta) =>
                {
                    foreach (var e in ta)
                    {
                        actual.Remove(new(e.PlaylistId, e.AudiotrackId));
                    }
                }
            );

        // Act
        await _repository.DeleteByPlaylist(MakeGuid(1));

        // Assert
        Assert.Equal(expectedPairs, actual);
    }

    public static IEnumerable<object[]> DeleteByAudiotrack_GetTestData()
    {
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(),
            new List<PlaylistAudiotrackDbModel>(),
            new List<Tuple<Guid, Guid>>(),
        };
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(
                [Tuple.Create(MakeGuid(1), MakeGuid(1))]
            ),
            new List<PlaylistAudiotrackDbModel>(
                [new(MakeGuid(1), MakeGuid(1))]
            ),
            new List<Tuple<Guid, Guid>>(),
        };
        yield return new object[]
        {
            new List<Tuple<Guid, Guid>>(
                [Tuple.Create(MakeGuid(2), MakeGuid(2))]
            ),
            new List<PlaylistAudiotrackDbModel>(
                [new(MakeGuid(2), MakeGuid(2))]
            ),
            new List<Tuple<Guid, Guid>>(
                [Tuple.Create(MakeGuid(2), MakeGuid(2))]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(DeleteByAudiotrack_GetTestData))]
    public async Task DeleteByAudiotrack_Ok(
        List<Tuple<Guid, Guid>> initial,
        List<PlaylistAudiotrackDbModel> returnedPairs,
        List<Tuple<Guid, Guid>> expectedPairs
    )
    {
        // Arrange
        List<Tuple<Guid, Guid>> actual = initial;

        _mockFactory.MockPlaylistsAudiotracksDbModel =
            MockDbContextFactory.SetupMockDbSet(returnedPairs);

        _mockFactory
            .MockContext.Setup(m => m.PlaylistsAudiotracks)
            .ReturnsDbSet(_mockFactory.MockPlaylistsAudiotracksDbModel.Object);
        _mockFactory
            .MockContext.Setup(s =>
                s.PlaylistsAudiotracks.RemoveRange(
                    It.IsAny<IEnumerable<PlaylistAudiotrackDbModel>>()
                )
            )
            .Callback(
                (IEnumerable<PlaylistAudiotrackDbModel> ta) =>
                {
                    foreach (var e in ta)
                    {
                        actual.Remove(
                            new(e.PlaylistId, e.AudiotrackId)
                        );
                    }
                }
            );

        // Act
        await _repository.DeleteByAudiotrack(MakeGuid(1));

        // Assert
        Assert.Equal(expectedPairs, actual);
    }

    public static IEnumerable<object[]> GetAllAudiotracksFromPlaylist_GetTestData()
    {
        yield return new object[] { new List<PlaylistAudiotrackDbModel>(), 0 };
        yield return new object[]
        {
            new List<PlaylistAudiotrackDbModel>(
                [new(MakeGuid(1), MakeGuid(1))]
            ),
            1,
        };
        yield return new object[]
        {
            new List<PlaylistAudiotrackDbModel>(
                [new(MakeGuid(2), MakeGuid(2))]
            ),
            0,
        };
    }

    [Theory]
    [MemberData(nameof(GetAllAudiotracksFromPlaylist_GetTestData))]
    public async Task GetAllAudiotracksFromPlaylist_ReturnsFound(
        List<PlaylistAudiotrackDbModel> returnedPairs,
        int expectedCount
    )
    {
        // Arrange
        var expectedPlaylistId = MakeGuid(1);
        _mockFactory
            .MockContext.Setup(m => m.PlaylistsAudiotracks)
            .ReturnsDbSet(returnedPairs);

        // Act
        var actual = await _repository.GetAllAudiotracksFromPlaylist(
            expectedPlaylistId
        );

        // Assert
        Assert.Equal(expectedCount, actual.Count);
    }

    public static IEnumerable<object[]> IsAudiotrackInPlaylist_GetTestData()
    {
        yield return new object[]
        {
            new List<PlaylistAudiotrackDbModel>(),
            false,
        };
        yield return new object[]
        {
            new List<PlaylistAudiotrackDbModel>(
                [new(MakeGuid(1), MakeGuid(1))]
            ),
            true,
        };
    }

    [Theory]
    [MemberData(nameof(IsAudiotrackInPlaylist_GetTestData))]
    public async Task IsAudiotrackInPlaylist_ReturnsResult(
        List<PlaylistAudiotrackDbModel> returnedPairs,
        bool expectedResult
    )
    {
        // Arrange
        _mockFactory
            .MockContext.Setup(x => x.PlaylistsAudiotracks)
            .ReturnsDbSet(returnedPairs);

        // Act
        var actual = await _repository.IsAudiotrackInPlaylist(
            MakeGuid(1),
            MakeGuid(1)
        );

        // Assert
        Assert.Equal(expectedResult, actual);
    }
}
