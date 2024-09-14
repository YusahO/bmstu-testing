using MewingPad.Common.Entities;

namespace MewingPad.Common.IRepositories;

public interface IScoreRepository
{
    Task AddScore(Score score);
    Task<Score> UpdateScore(Score score);
    Task DeleteScore(Guid authorId, Guid audiotrackId);
    Task<Score?> GetScoreByPrimaryKey(Guid authorId, Guid audiotrackId);
    Task<List<Score>> GetAllScores();
    Task<List<Score>> GetAudiotrackScores(Guid audiotrackId);
}