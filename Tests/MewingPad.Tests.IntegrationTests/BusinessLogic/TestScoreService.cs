using MewingPad.Common.Exceptions;
using MewingPad.Database.Context;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Services.ScoreService;
using MewingPad.Tests.Factories.Core;

namespace MewingPad.Tests.IntegrationTests.BusinessLogic;

[Collection("Test Database")]
public class ScoreServiceIntegrationTest : BaseServiceTestClass
{
    private readonly MewingPadDbContext _context;
    private readonly ScoreService _service;
    private readonly ScoreRepository _scoreRepository;
    private readonly AudiotrackRepository _audiotrackRepository;

    private async Task AddDefaultScore()
    {
        await _context.Scores.AddAsync(
            new ScoreDbModelBuilder()
                .WithAuthorId(DefaultUserId)
                .WithAudiotrackId(DefaultAudiotrackId)
                .WithValue(4)
                .Build()
        );
        await _context.SaveChangesAsync();
    }

    public ScoreServiceIntegrationTest(DatabaseFixture fixture)
        : base(fixture)
    {
        _context = Fixture.CreateContext();

        _scoreRepository = new(_context);
        _audiotrackRepository = new(_context);

        _service = new(_scoreRepository, _audiotrackRepository);
    }

    [Fact]
    public async Task CreateScore_CreateUnique_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var score = ScoreFabric.Create(DefaultAudiotrackId, DefaultUserId, 2);

        // Act
        await _service.CreateScore(score);

        // Assert
        var actual = (from s in _context.Scores select s).ToList();
        Assert.Single(actual);
        Assert.Equal(score.AuthorId, actual[0].AuthorId);
        Assert.Equal(score.AudiotrackId, actual[0].AudiotrackId);
    }

    [Fact]
    public async Task CreateScore_CreateExistent_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultScore();

        var score = ScoreFabric.Create(DefaultAudiotrackId, DefaultUserId, 2);

        // Act
        async Task Action() => await _service.CreateScore(score);

        // Assert
        await Assert.ThrowsAsync<ScoreExistsException>(Action);
    }

    [Fact]
    public async Task UpdateScore_UpdateExistent_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        const int expectedScore = 2;

        using (var context = Fixture.CreateContext())
        {
            await context.Scores.AddAsync(
                new ScoreDbModelBuilder()
                    .WithAuthorId(DefaultUserId)
                    .WithAudiotrackId(DefaultAudiotrackId)
                    .WithValue(4)
                    .Build()
            );
            await context.SaveChangesAsync();
        }

        var score = ScoreFabric.Create(
            DefaultAudiotrackId,
            DefaultUserId,
            expectedScore
        );

        // Act
        var actual = await _service.UpdateScore(score);

        // Assert
        Assert.Equal(expectedScore, actual.Value);
    }

    [Fact]
    public async Task UpdateScore_UpdateNonexistent_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var score = ScoreFabric.Create(DefaultAudiotrackId, DefaultUserId, 2);

        // Act
        async Task Action() => await _service.UpdateScore(score);

        // Assert
        await Assert.ThrowsAsync<ScoreNotFoundException>(Action);
    }

    [Fact]
    public async Task GetScoreByPrimaryKey_ScoreWithPkExists_ReturnsScore()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultScore();

        var expectedAuthorId = DefaultUserId;
        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act
        var actual = await _service.GetScoreByPrimaryKey(
            expectedAuthorId,
            expectedAudiotrackId
        );

        // Assert
        Assert.Equal(expectedAuthorId, actual.AuthorId);
        Assert.Equal(expectedAudiotrackId, actual.AudiotrackId);
    }

    [Fact]
    public async Task GetScoreByPrimaryKey_NoScoreWithPk_Error()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultScore();

        // Act
        async Task Action() =>
            await _service.GetScoreByPrimaryKey(new(), new());

        // Assert
        await Assert.ThrowsAsync<ScoreNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAudiotrackScores_AudiotrackAndScoresExist_ReturnsScores()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultScore();

        var expectedAudiotrackId = DefaultAudiotrackId;

        // Act
        var actual = await _service.GetAudiotrackScores(DefaultAudiotrackId);

        // Assert
        Assert.Single(actual);
        Assert.Equal(expectedAudiotrackId, actual[0].AudiotrackId);
    }

    [Fact]
    public async Task GetAudiotrackScores_AudiotrackHasNoScores_ReturnsEmpty()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        // Act
        var actual = await _service.GetAudiotrackScores(DefaultAudiotrackId);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotrackScores_NoAudiotrackWithId_Error()
    {
        // Arrange

        // Act
        async Task Action() => await _service.GetAudiotrackScores(new());

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }
}
