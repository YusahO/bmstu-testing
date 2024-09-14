using MewingPad.Common.Entities;

namespace MewingPad.Common.IRepositories;

public interface ITagAudiotrackRepository
{
    Task DeleteByTag(Guid tagId);
    Task DeleteByAudiotrack(Guid audiotrackId);
    Task<List<Tag>> GetAudiotrackTags(Guid audiotrackId);
    Task<List<Audiotrack>> GetAudiotracksWithTags(List<Guid> tagIds);
    Task AssignTagToAudiotrack(Guid audiotrackId, Guid tagId);
    Task RemoveTagFromAudiotrack(Guid audiotrackId, Guid tagId);
}