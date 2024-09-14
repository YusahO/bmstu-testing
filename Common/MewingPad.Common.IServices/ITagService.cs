using MewingPad.Common.Entities;
namespace MewingPad.Services.TagService;

public interface ITagService
{
    Task CreateTag(Tag tag);
    Task<Tag> UpdateTagName(Guid tagId, string tagName);
    Task DeleteTag(Guid tagId);
    Task<Tag> GetTagById(Guid tagId);
    Task<List<Tag>> GetAllTags();
    Task<List<Tag>> GetAudiotrackTags(Guid audiotrackId);
    Task<List<Audiotrack>> GetAudiotracksWithTags(List<Guid> tagId);
    Task AssignTagToAudiotrack(Guid audiotrackId, Guid tagId);
    Task DeleteTagFromAudiotrack(Guid audiotrackId, Guid tagId);
}
