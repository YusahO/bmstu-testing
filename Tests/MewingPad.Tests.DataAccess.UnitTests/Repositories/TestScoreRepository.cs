using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using Moq.EntityFrameworkCore;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

public class TestScoreRepository : BaseRepositoryTestClass
{
    private readonly ScoreRepository _repository;
    private readonly MockDbContextFactory _mockFactory;

    public TestScoreRepository()
    {
        _mockFactory = new MockDbContextFactory();
        _repository = new(_mockFactory.MockContext.Object);
    }

    private static Score CreateScoreCoreModel(
        Guid authorId,
        Guid audiotrackId,
        int value
    )
    {
        return new ScoreCoreModelBuilder()
            .WithAuthorId(authorId)
            .WithAudiotrackId(audiotrackId)
            .WithValue(value)
            .Build();
    }

    private static ScoreDbModel CreateScoreDbo(
        Guid authorId,
        Guid audiotrackId,
        int value
    )
    {
        return new ScoreDbModelBuilder()
            .WithAuthorId(authorId)
            .WithAudiotrackId(audiotrackId)
            .WithValue(value)
            .Build();
    }

    private static ScoreDbModel CreateScoreDboFromCore(Score score)
    {
        return CreateScoreDbo(score.AuthorId, score.AudiotrackId, score.Value);
    }

    [Fact]
    public async void AddScore_AddUnique_Ok()
    {
        List<ScoreDbModel> actual = [];
        var score = CreateScoreCoreModel(MakeGuid(1), MakeGuid(1), 3);
        var scoreDbo = CreateScoreDboFromCore(score);

        _mockFactory
            .MockScoresDbSet.Setup(s =>
                s.AddAsync(It.IsAny<ScoreDbModel>(), default)
            )
            .Callback<ScoreDbModel, CancellationToken>(
                (s, token) => actual.Add(s)
            );

        // Act
        await _repository.AddScore(score);

        // Assert
        Assert.Single(actual);
        Assert.Equal(score.Value, actual[0].Value);
        Assert.Equal(score.AuthorId, actual[0].AuthorId);
        Assert.Equal(score.AudiotrackId, actual[0].AudiotrackId);
    }

    [Fact]
    public async void AddScore_AddScoreWithSameId_Error()
    {
        // Arrange
        _mockFactory
            .MockScoresDbSet.Setup(s =>
                s.AddAsync(It.IsAny<ScoreDbModel>(), default)
            )
            .Callback<ScoreDbModel, CancellationToken>(
                (s, token) => throw new RepositoryException()
            );

        // Act
        async Task Action() => await _repository.AddScore(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdateScore_UpdateExisting_Ok()
    {
        // Arrange
        Guid expectedAuthorId = MakeGuid(1);
        Guid expectedAudiotrackId = MakeGuid(1);
        const int expectedValue = 4;

        var score = CreateScoreCoreModel(
            expectedAuthorId,
            expectedAudiotrackId,
            expectedValue
        );
        var scoreDbo = CreateScoreDboFromCore(score);
        List<ScoreDbModel> scoreDbos = [scoreDbo];

        _mockFactory
            .MockScoresDbSet.Setup(s => s.Update(It.IsAny<ScoreDbModel>()))
            .Callback((ScoreDbModel s) => scoreDbos[0].Value = expectedValue);

        // Act
        await _repository.UpdateScore(score);

        // Assert
        Assert.Single(scoreDbos);
        Assert.Equal(expectedValue, scoreDbos[0].Value);
        Assert.Equal(score.AuthorId, scoreDbos[0].AuthorId);
        Assert.Equal(score.AudiotrackId, scoreDbos[0].AudiotrackId);
    }

    [Fact]
    public async void UpdateScore_UpdateNonexistent_Error()
    {
        // Arrange
        _mockFactory
            .MockScoresDbSet.Setup(s => s.Update(It.IsAny<ScoreDbModel>()))
            .Callback((ScoreDbModel s) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.UpdateScore(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    public static IEnumerable<object[]> GetScoreByPrimaryKey_GetTestData()
    {
        yield return new object[] { default(ScoreDbModel)!, default(Score)! };
        yield return new object[]
        {
            CreateScoreDbo(MakeGuid(1), MakeGuid(1), 3),
            CreateScoreCoreModel(MakeGuid(1), MakeGuid(1), 3),
        };
    }

    [Theory]
    [MemberData(nameof(GetScoreByPrimaryKey_GetTestData))]
    public async void GetScoreByPrimaryKey_ReturnsFound(
        ScoreDbModel returnedScoreDbo,
        Score expectedScore
    )
    {
        // Arrange
        _mockFactory
            .MockScoresDbSet.Setup(x => x.FindAsync(It.IsAny<object?[]?>()))
            .ReturnsAsync(returnedScoreDbo);

        // Act
        var actual = await _repository.GetScoreByPrimaryKey(new(), new());

        // Assert
        Assert.Equal(expectedScore, actual);
    }

    public static IEnumerable<object[]> GetAllScores_GetTestData()
    {
        yield return new object[]
        {
            new List<ScoreDbModel>(),
            new List<Score>(),
        };
        yield return new object[]
        {
            new List<ScoreDbModel>(
                [CreateScoreDbo(MakeGuid(1), MakeGuid(1), 3)]
            ),
            new List<Score>(
                [CreateScoreCoreModel(MakeGuid(1), MakeGuid(1), 3)]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetAllScores_GetTestData))]
    public async void GetAllScores_ReturnsFound(
        List<ScoreDbModel> scoreDbos,
        List<Score> expectedScores
    )
    {
        // Arrange
        _mockFactory.MockContext.Setup(x => x.Scores).ReturnsDbSet(scoreDbos);

        // Act
        var actual = await _repository.GetAllScores();

        // Assert
        Assert.Equal(expectedScores, actual);
    }

    public static IEnumerable<object[]> GetAudiotrackScores_GetTestData()
    {
        yield return new object[]
        {
            new List<ScoreDbModel>(),
            new List<Score>(),
        };
        yield return new object[]
        {
            new List<ScoreDbModel>(
                [
                    CreateScoreDbo(MakeGuid(1), MakeGuid(1), 3),
                    CreateScoreDbo(MakeGuid(1), MakeGuid(2), 3),
                ]
            ),
            new List<Score>(
                [CreateScoreCoreModel(MakeGuid(1), MakeGuid(1), 3)]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetAudiotrackScores_GetTestData))]
    public async void GetAudiotrackScores_ReturnsFound(
        List<ScoreDbModel> scoreDbos,
        List<Score> expectedScores
    )
    {
        // Arrange
        _mockFactory.MockContext.Setup(x => x.Scores).ReturnsDbSet(scoreDbos);

        // Act
        var actual = await _repository.GetAudiotrackScores(MakeGuid(1));

        // Assert
        Assert.Equal(expectedScores, actual);
    }
}
