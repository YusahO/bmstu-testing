using MewingPad.Common.Entities;

namespace MewingPad.Services.CommentaryService;

public interface ICommentaryService
{
    Task CreateCommentary(Commentary commentary);
    Task<Commentary> UpdateCommentary(Commentary commentary);
    Task DeleteCommentary(Guid commentaryId);
    Task<Commentary> GetCommentaryById(Guid commentaryId);
    Task<List<Commentary>> GetAudiotrackCommentaries(Guid audiotrackId);
}