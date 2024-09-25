using MewingPad.Common.Exceptions;
using MewingPad.Services.ScoreService;
using MewingPad.Tests.Factories.Core;

namespace MewingPad.Tests.UnitTests.BusinessLogic.Services;

public class ScoreServiceUnitTest : BaseServiceTestClass
{
    private readonly ScoreService _scoreService;
    private readonly Mock<IScoreRepository> _mockScoreRepository = new();
    private readonly Mock<IAudiotrackRepository> _mockAudiotrackRepository =
        new();

    public ScoreServiceUnitTest()
    {
        _scoreService = new ScoreService(
            _mockScoreRepository.Object,
            _mockAudiotrackRepository.Object
        );
    }

    [Fact]
    public async Task CreateScore_CreateUnique_Ok()
    {
        // Arrange
        var expectedScore = ScoreFabric.Create(MakeGuid(1), MakeGuid(1), 3);
        List<Score> scores = [];

        _mockScoreRepository
            .Setup(s =>
                s.GetScoreByPrimaryKey(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(default(Score)!);
        _mockScoreRepository
            .Setup(s => s.AddScore(It.IsAny<Score>()))
            .Callback((Score s) => scores.Add(expectedScore));

        // Act
        await _scoreService.CreateScore(new(expectedScore));

        // Assert
        Assert.Single(scores);
    }

    [Fact]
    public async Task CreateScore_CreateExistent_Error()
    {
        // Arrange
        List<Score> scores = [];
        _mockScoreRepository
            .Setup(s =>
                s.GetScoreByPrimaryKey(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(default(Score)!);

        // Act
        await _scoreService.CreateScore(ScoreFabric.CreateEmpty());

        // Assert
        Assert.Empty(scores);
        _mockScoreRepository.Verify(
            s => s.AddScore(It.IsAny<Score>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateScore_UpdateExistent_Ok()
    {
        // Arrange
        var oldScore = ScoreFabric.Create(MakeGuid(1), MakeGuid(1), 3);
        List<Score> scores = [oldScore];
        var expectedScore = new Score(oldScore) { Value = 4 };

        _mockScoreRepository
            .Setup(s =>
                s.GetScoreByPrimaryKey(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(ScoreFabric.CreateEmpty());
        _mockScoreRepository
            .Setup(s => s.UpdateScore(It.IsAny<Score>()))
            .Callback(
                (Score s) =>
                {
                    scores[0].Value = expectedScore.Value;
                }
            );

        // Act
        var actual = await _scoreService.UpdateScore(new(expectedScore));

        // Assert
        Assert.Single(scores);
        Assert.Equal(expectedScore, actual);
    }

    [Fact]
    public async Task UpdateScore_UpdateWithInvalidValue_Error()
    {
        // Arrange
        var oldScore = ScoreFabric.Create(MakeGuid(1), MakeGuid(1), 3);
        List<Score> scores = [oldScore];

        _mockScoreRepository
            .Setup(s =>
                s.GetScoreByPrimaryKey(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(ScoreFabric.CreateEmpty());
        _mockScoreRepository
            .Setup(s => s.UpdateScore(It.IsAny<Score>()))
            .Callback(
                (Score s) =>
                {
                    scores[0].Value = 6;
                }
            );

        // Act
        async Task Action() => await _scoreService.UpdateScore(new(oldScore));

        // Assert
        await Assert.ThrowsAsync<ScoreInvalidValueException>(Action);
        Assert.Single(scores);
    }

    [Fact]
    public async Task UpdateScore_UpdateNonexistent_Error()
    {
        // Arrange
        _mockScoreRepository
            .Setup(s =>
                s.GetScoreByPrimaryKey(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(default(Score)!);

        // Act
        async Task Action() => await _scoreService.UpdateScore(new());

        // Assert
        await Assert.ThrowsAsync<ScoreNotFoundException>(Action);
    }

    [Fact]
    public async Task GetScoreByPrimaryKey_ScoreWithPkExists_ReturnsScore()
    {
        // Arrange
        var expectedScore = ScoreFabric.Create(MakeGuid(1), MakeGuid(1), 3);

        _mockScoreRepository
            .Setup(s =>
                s.GetScoreByPrimaryKey(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(new Score(expectedScore));

        // Act
        var actual = await _scoreService.GetScoreByPrimaryKey(
            expectedScore.AuthorId,
            expectedScore.AudiotrackId
        );

        // Assert
        Assert.Equal(expectedScore, actual);
    }

    [Fact]
    public async Task GetScoreByPrimaryKey_NoScoreWithPk_Error()
    {
        // Arrange
        _mockScoreRepository
            .Setup(s =>
                s.GetScoreByPrimaryKey(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .ReturnsAsync(default(Score)!);

        // Act
        async Task Action() =>
            await _scoreService.GetScoreByPrimaryKey(new(), new());

        // Assert
        await Assert.ThrowsAsync<ScoreNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAudiotrackScores_AudiotrackAndScoresExist_ReturnsScores()
    {
        // Arrange
        List<Score> expectedScores =
        [
            ScoreFabric.Create(MakeGuid(1), MakeGuid(1), 3),
        ];

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(AudiotrackFabric.CreateEmpty());
        _mockScoreRepository
            .Setup(s => s.GetAudiotrackScores(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Score>(expectedScores));

        // Act
        var actual = await _scoreService.GetAudiotrackScores(new());

        // Assert
        Assert.Equal(expectedScores, actual);
    }

    [Fact]
    public async Task GetAudiotrackScores_AudiotrackHasNoScores_ReturnsEmpty()
    {
        // Arrange

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(AudiotrackFabric.CreateEmpty());
        _mockScoreRepository
            .Setup(s => s.GetAudiotrackScores(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // Act
        var actual = await _scoreService.GetAudiotrackScores(new());

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotrackScores_NoAudiotrackWithId_Error()
    {
        // Arrange

        _mockAudiotrackRepository
            .Setup(s => s.GetAudiotrackById(It.IsAny<Guid>()))
            .ReturnsAsync(default(Audiotrack)!);

        // Act
        async Task Action() => await _scoreService.GetAudiotrackScores(new());

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }
}
