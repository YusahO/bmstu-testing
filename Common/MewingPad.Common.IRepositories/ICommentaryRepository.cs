using MewingPad.Common.Entities;

namespace MewingPad.Common.IRepositories;

public interface ICommentaryRepository
{
    Task AddCommentary(Commentary commentary);
    Task<Commentary> UpdateCommentary(Commentary commentary);
    Task DeleteCommentary(Guid commentaryId);
    Task<Commentary?> GetCommentaryById(Guid commentaryId);
    Task<List<Commentary>> GetAudiotrackCommentaries(Guid audiotrackId);
}