using MewingPad.Common.Entities;

namespace MewingPad.Services.ScoreService;

public interface IScoreService
{
    Task CreateScore(Score score);
    Task<Score> UpdateScore(Score score);
    Task<Score?> GetScoreByPrimaryKey(Guid authorId, Guid audiotrackId);
    Task<List<Score>> GetAudiotrackScores(Guid audiotrackId);
}