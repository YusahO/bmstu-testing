using MewingPad.Common.Entities;

namespace MewingPad.Common.IRepositories;

public interface ITagRepository
{
    Task AddTag(Tag tag);
    Task<Tag> UpdateTag(Tag tag);
    Task DeleteTag(Guid tagId);
    Task<Tag?> GetTagById(Guid tagId);
    Task<List<Tag>> GetAllTags();
}