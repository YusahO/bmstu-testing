using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;

namespace MewingPad.Tests.IntegrationTests.DataAccess;

[Collection("Test Database")]
public class TestScoreRepository : BaseRepositoryTestClass
{
    private readonly ScoreRepository _repository;

    public TestScoreRepository(DatabaseFixture fixture)
        : base(fixture)
    {
        _repository = new(Fixture.CreateContext());
    }

    [Fact]
    public async void AddScore_AddSingle_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedAuthorId = DefaultUserId;
        var expectedAudiotrackId = DefaultAudiotrackId;
        const int expectedValue = 3;

        var score = new ScoreCoreModelBuilder()
            .WithAuthorId(expectedAuthorId)
            .WithAudiotrackId(expectedAudiotrackId)
            .WithValue(expectedValue)
            .Build();

        // Act
        await _repository.AddScore(score);

        // Assert
        var actual = (from a in context.Scores select a).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedAuthorId, actual[0].AuthorId);
        Assert.Equal(expectedAudiotrackId, actual[0].AudiotrackId);
        Assert.Equal(expectedValue, actual[0].Value);
    }

    [Fact]
    public async void AddScore_AddScoreWithSameId_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var score = new ScoreCoreModelBuilder()
            .WithAuthorId(DefaultUserId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .WithValue(3)
            .Build();
        await context.Scores.AddAsync(
            new ScoreDbModelBuilder()
                .WithAuthorId(score.AuthorId)
                .WithAudiotrackId(score.AudiotrackId)
                .WithValue(score.Value)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        async Task Action() => await _repository.AddScore(score);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    // [Fact]
    // public async void UpdateScore_UpdateExisting_Ok()
    // {
    //     // Arrange
    //     await AddDefaultUserWithPlaylist();
    //     await AddDefaultAudiotrack();

    //     var expectedAuthorId = DefaultUserId;
    //     var expectedAudiotrackId = DefaultAudiotrackId;
    //     const int expectedValue = 1;

    //     var score = new ScoreCoreModelBuilder()
    //         .WithAuthorId(expectedAuthorId)
    //         .WithAudiotrackId(expectedAudiotrackId)
    //         .WithValue(3)
    //         .Build();

    //     using (var context = Fixture.CreateContext())
    //     {
    //         await context.Scores.AddAsync(
    //             new ScoreDbModelBuilder()
    //                 .WithAuthorId(score.AuthorId)
    //                 .WithAudiotrackId(score.AudiotrackId)
    //                 .WithValue(score.Value)
    //                 .Build()
    //         );
    //         await context.SaveChangesAsync();
    //     }

    //     score.Value = expectedValue;

    //     // Act
    //     await _repository.UpdateScore(score);

    //     // Assert
    //     using (var context = Fixture.CreateContext())
    //     {
    //         var actual = (from a in context.Scores select a).ToList();
    //         Assert.Single(actual);
    //         Assert.Equal(expectedValue, actual[0].Value);
    //         Assert.Equal(expectedAuthorId, actual[0].AuthorId);
    //         Assert.Equal(expectedAudiotrackId, actual[0].AudiotrackId);
    //     }
    // }

    [Fact]
    public async void UpdateScore_UpdateNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var score = new ScoreCoreModelBuilder()
            .WithAuthorId(DefaultUserId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .WithValue(3)
            .Build();

        // Act
        async Task Action() => await _repository.UpdateScore(score);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void GetScoreByPrimaryKey_ScoreExists_ReturnsScore()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedAuthorId = DefaultUserId;
        var expectedAudiotrackId = MakeGuid(3);
        const int expectedValue = 3;

        Guid[] audiotrackIds = Enumerable
            .Range(1, 3)
            .Select(i => MakeGuid(Convert.ToByte(i)))
            .ToArray();
        foreach (var id in audiotrackIds)
        {
            await AddAudiotrackWithId(id);
        }
        for (byte i = 0; i < 3; ++i)
        {
            await context.Scores.AddAsync(
                new ScoreDbModelBuilder()
                    .WithAuthorId(DefaultUserId)
                    .WithAudiotrackId(audiotrackIds[i])
                    .WithValue(i + 1)
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetScoreByPrimaryKey(
            expectedAuthorId,
            expectedAudiotrackId
        );

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedAudiotrackId, actual.AudiotrackId);
        Assert.Equal(expectedAuthorId, actual.AuthorId);
        Assert.Equal(expectedValue, actual.Value);
    }

    [Fact]
    public async void GetScoreByPrimaryKey_NoScoreWithSuchId_ReturnsNull()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedAudiotrackId = MakeGuid(2);

        await context.Scores.AddAsync(
            new ScoreDbModelBuilder()
                .WithAuthorId(DefaultUserId)
                .WithAudiotrackId(DefaultAudiotrackId)
                .WithValue(4)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetScoreByPrimaryKey(
            DefaultUserId,
            expectedAudiotrackId
        );

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetScoreByPrimaryKey_NoScores_ReturnsNull()
    {
        // Arrange

        // Act
        var actual = await _repository.GetScoreByPrimaryKey(
            new Guid(),
            new Guid()
        );

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetAllScores_ScoresExist_ReturnsScores()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid[] audiotrackIds = Enumerable
            .Range(1, 3)
            .Select(i => MakeGuid(Convert.ToByte(i)))
            .ToArray();
        foreach (var id in audiotrackIds)
        {
            await AddAudiotrackWithId(id);
        }
        for (byte i = 0; i < 3; ++i)
        {
            await context.Scores.AddAsync(
                new ScoreDbModelBuilder()
                    .WithAuthorId(DefaultUserId)
                    .WithAudiotrackId(audiotrackIds[i])
                    .WithValue(i + 1)
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAllScores();

        // Assert
        Assert.Equal(3, actual.Count);
    }

    [Fact]
    public async void GetAllScores_NoScores_ReturnsEmpty()
    {
        // Arrange

        // Act
        var actual = await _repository.GetAllScores();

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async void GetAudiotrackScores_AudiotrackScoresExist_ReturnsScores()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var anotherUserId = MakeGuid(2);
        await AddDefaultUserWithPlaylist();
        await AddUserWithPlaylistWithIds(anotherUserId, MakeGuid(2));
        await AddDefaultAudiotrack();

        var expectedAudiotrackId = DefaultAudiotrackId;

        await context.Scores.AddAsync(
            new ScoreDbModelBuilder()
                .WithAuthorId(DefaultUserId)
                .WithAudiotrackId(DefaultAudiotrackId)
                .WithValue(3)
                .Build()
        );
        await context.Scores.AddAsync(
            new ScoreDbModelBuilder()
                .WithAuthorId(anotherUserId)
                .WithAudiotrackId(DefaultAudiotrackId)
                .WithValue(2)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotrackScores(
            expectedAudiotrackId
        );

        // Assert
        Assert.Equal(2, actual.Count);
        Assert.All(
            actual,
            s => Assert.Equal(expectedAudiotrackId, s.AudiotrackId)
        );
    }

    [Fact]
    public async void GetAudiotrackScores_NoAudiotrack_ReturnsEmpty()
    {
        // Arrange

        // Act
        var actual = await _repository.GetAudiotrackScores(DefaultAudiotrackId);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async void GetAudiotrackScores_NoScores_ReturnsEmpty()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        // Act
        var actual = await _repository.GetAudiotrackScores(DefaultAudiotrackId);

        // Assert
        Assert.Empty(actual);
    }
}
